namespace APPLICATION_BACKEND.Interfaces
{
    public interface IPasswordEncryptionService
    {
        string EncryptPassword(string password);
        string DecryptPassword(string encryptedPassword);
        bool VerifyPassword(string password, string encryptedPassword);
    }
}
