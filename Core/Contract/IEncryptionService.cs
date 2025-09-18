namespace Core.Contract
{
    public interface IEncryptionService
    {
        string Encrypt(string plaintext);
        string Decrypt(string ciphertext);
    }
}
