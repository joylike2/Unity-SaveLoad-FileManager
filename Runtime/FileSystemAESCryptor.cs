using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LifeLogs.FileSystem.Utils {
    internal static class FileSystemAESCryptor {
        private static byte[] GenerateKey(string userKey) {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(userKey)); //32바이트로 변환
        }
        private static byte[] GenerateIV(string userKey) {
            using MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(userKey));    //16바이트로 변환
        }

        /// <summary> 암호화 처리 </summary>
        public static string Encrypt(string plainData, string userKey) {
            using var aes = Aes.Create();
            aes.Key = GenerateKey(userKey);
            aes.IV = GenerateIV(userKey);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(plainData);
            sw.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary> 복호화 처리 </summary>
        public static string Decrypt(string cipherData, string userKey) {
            using var aes = Aes.Create();
            aes.Key = GenerateKey(userKey);
            aes.IV = GenerateIV(userKey);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherData));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}