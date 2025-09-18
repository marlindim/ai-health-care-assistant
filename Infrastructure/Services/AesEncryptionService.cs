using Core.Contract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _fg_key;
        private readonly byte[] _iv;
        public AesEncryptionService(IConfiguration _Iconfig)
        {
            var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // 256-bit key
            var iv = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));  // 128-bit IV
            Console.WriteLine($"Key: {key}");
            Console.WriteLine($"IV: {iv}");

            _fg_key = Convert.FromBase64String(_Iconfig["Encryption:Key"]!);
            _iv = Convert.FromBase64String(_Iconfig["Encryption:IV"]!);
        }
        public string Decrypt(string ciphertext)
        {
            using var aes = Aes.Create();
            aes.Key = _fg_key;
            aes.IV = _iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(ciphertext));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        public string Encrypt(string plaintext)
        {
            using var aes = Aes.Create();
            aes.Key = _fg_key;
            aes.IV = _iv;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plaintext);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
