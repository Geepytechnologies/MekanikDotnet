using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace MekanikApi.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string publicKeyPem = Environment.GetEnvironmentVariable("");

        public string EncryptData(object dataToEncrypt)
        {
            string jsonData = JsonConvert.SerializeObject(dataToEncrypt);
            byte[] publicKeyBytes = Convert.FromBase64String(publicKeyPem);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

            byte[] dataToEncryptBytes = Encoding.UTF8.GetBytes(jsonData);
            byte[] encryptedData = rsa.Encrypt(dataToEncryptBytes, RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(encryptedData);
        }
    }

    public interface IEncryptionService
    {
        string EncryptData(object dataToEncrypt);
    }
}