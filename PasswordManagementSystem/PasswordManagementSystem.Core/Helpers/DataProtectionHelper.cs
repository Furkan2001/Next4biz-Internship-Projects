using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace PasswordManagementSystem.Core.Helpers
{
    public class DataProtectionHelper
    {
        private readonly string key = "your-256-bit-long-key1234567890t"; // 32 bytes key for AES-256

        public DataProtectionHelper()
        {
        }

        public string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;

                using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
                {
                    byte[] iv = new byte[aes.IV.Length];
                    memoryStream.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] plainBytes = new byte[cipherBytes.Length - aes.IV.Length];
                        int decryptedCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

                        return Encoding.UTF8.GetString(plainBytes, 0, decryptedCount);
                    }
                }
            }
        }
    }
}
