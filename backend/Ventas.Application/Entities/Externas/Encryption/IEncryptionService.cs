namespace Ventas.Application.Entities.Externas.Encryption
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
