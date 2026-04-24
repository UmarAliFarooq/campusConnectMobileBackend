using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Services
{
    public class TokenService
    {
        private readonly string _secretKey;
        private readonly int _tokenExpiryHours = 24; // Token valid for 24 hours

        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration["EncryptionSettings:SecretKey"] ?? throw new ArgumentNullException("Secret key not configured");
        }

        public string GenerateToken(UserResponseDto user)
        {
            try
            {
                // Create token payload with user info
                var tokenPayload = new
                {
                    UserId = user.UserId,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = user.RoleName,
                    ExpiresAt = DateTime.UtcNow.AddHours(_tokenExpiryHours),
                    IssuedAt = DateTime.UtcNow
                };

                // Serialize payload to JSON
                string payloadJson = System.Text.Json.JsonSerializer.Serialize(tokenPayload);
                
                // Create a simple token using base64 encoding with timestamp
                string tokenData = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson))}.{Guid.NewGuid()}";
                
                // Add signature using HMAC-SHA256
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
                {
                    byte[] signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenData));
                    string signatureBase64 = Convert.ToBase64String(signature);
                    
                    return $"{tokenData}.{signatureBase64}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate token: {ex.Message}");
            }
        }

        public AuthResponseDto? ValidateToken(string token)
        {
            try
            {
                string[] parts = token.Split('.');
                if (parts.Length != 3)
                    return null;

                string payloadBase64 = parts[0];
                string guid = parts[1];
                string signatureBase64 = parts[2];

                // Verify signature
                string tokenData = $"{payloadBase64}.{guid}";
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
                {
                    byte[] expectedSignature = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenData));
                    string expectedSignatureBase64 = Convert.ToBase64String(expectedSignature);

                    if (signatureBase64 != expectedSignatureBase64)
                        return null;
                }

                // Decode payload
                byte[] payloadBytes = Convert.FromBase64String(payloadBase64);
                string payloadJson = Encoding.UTF8.GetString(payloadBytes);

                var tokenPayload = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(payloadJson);
                
                // Check if token is expired
                DateTime expiresAt = tokenPayload.GetProperty("ExpiresAt").GetDateTime();
                if (DateTime.UtcNow > expiresAt)
                    return null;

                // Extract user info
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = tokenPayload.GetProperty("UserId").GetInt64(),
                    EmailAddress = tokenPayload.GetProperty("EmailAddress").GetString() ?? string.Empty,
                    FirstName = tokenPayload.GetProperty("FirstName").GetString() ?? string.Empty,
                    LastName = tokenPayload.GetProperty("LastName").GetString() ?? string.Empty,
                    RoleName = tokenPayload.GetProperty("RoleName").GetString() ?? string.Empty,
                    ExpiresAt = expiresAt
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
