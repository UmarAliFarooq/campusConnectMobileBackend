using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Services
{
    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private readonly string _secretKey;
        private readonly int _maxLength = 200;

        public PasswordEncryptionService(IConfiguration configuration)
        {
            _secretKey = configuration["EncryptionSettings:SecretKey"] ?? throw new ArgumentNullException("Secret key not configured");
        }

        public string EncryptPassword(string password)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    // Generate a random IV for each encryption
                    byte[] iv = aes.IV;
                    
                    // Derive the key from the secret key
                    using (var pbkdf2 = new Rfc2898DeriveBytes(_secretKey, Encoding.UTF8.GetBytes(_secretKey), 10000))
                    {
                        aes.Key = pbkdf2.GetBytes(32); // 256 bits
                    }
                    
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    {
                        // Prepend the IV to the encrypted data
                        msEncrypt.Write(iv, 0, iv.Length);
                        
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(password);
                        }
                        
                        string encrypted = Convert.ToBase64String(msEncrypt.ToArray());
                        
                        // Ensure the encrypted password doesn't exceed 200 characters
                        if (encrypted.Length > _maxLength)
                        {
                            encrypted = encrypted.Substring(0, _maxLength);
                        }
                        
                        return encrypted;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to encrypt password: {ex.Message}");
            }
        }

        public string DecryptPassword(string encryptedPassword)
        {
            try
            {
                byte[] fullCipher = Convert.FromBase64String(encryptedPassword);
                
                using (Aes aes = Aes.Create())
                {
                    // Extract the IV from the beginning of the cipher
                    byte[] iv = new byte[aes.BlockSize / 8];
                    byte[] cipher = new byte[fullCipher.Length - iv.Length];
                    
                    Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                    Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);
                    
                    // Derive the key from the secret key
                    using (var pbkdf2 = new Rfc2898DeriveBytes(_secretKey, Encoding.UTF8.GetBytes(_secretKey), 10000))
                    {
                        aes.Key = pbkdf2.GetBytes(32); // 256 bits
                    }
                    
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(cipher))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decrypt password: {ex.Message}");
            }
        }

        public bool VerifyPassword(string password, string encryptedPassword)
        {
            try
            {
                string decryptedPassword = DecryptPassword(encryptedPassword);
                return password == decryptedPassword;
            }
            catch
            {
                return false;
            }
        }
    }
}
