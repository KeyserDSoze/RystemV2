using Rystem.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Security.Cryptography
{
    public static class Crypto
    {
        public static class Aes
        {
            public static void Configure(string password, string saltKey, string vIKey)
            {
                Password = password;
                SaltKeyAsBytes = Encoding.ASCII.GetBytes(saltKey);
                VIKeyAsBytes = Encoding.ASCII.GetBytes(vIKey);
                IsConfigured = true;
            }
            internal static bool IsConfigured { get; set; }
            private static string Password;
            private static byte[] SaltKeyAsBytes;
            private static byte[] VIKeyAsBytes;
            private static byte[] KeyAsBytes => new Rfc2898DeriveBytes(Password, SaltKeyAsBytes).GetBytes(256 / 8);
            internal static RijndaelManaged SymmetricKey => new() { Mode = CipherMode.CBC, Padding = PaddingMode.ISO10126 };
            internal static ICryptoTransform Decryptor => SymmetricKey.CreateDecryptor(KeyAsBytes, VIKeyAsBytes);
            internal static ICryptoTransform Encryptor => SymmetricKey.CreateEncryptor(KeyAsBytes, VIKeyAsBytes);
        }
        public static string ToHash(this string message)
        {
            using SHA256 mySHA256 = SHA256.Create("SHA256");
            byte[] bytes = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(message));
            StringBuilder stringBuilder = new();
            foreach (var @byte in bytes)
                stringBuilder.Append(@byte.ToString("x2"));
            return stringBuilder.ToString();
        }
        public static string ToHash<T>(this T message)
            => message.ToJson().ToHash();
        public static string Encrypt(this string message)
        {
            if (!Aes.IsConfigured)
                throw new ArgumentException("Please use Crypto.Aes.Configure() method to configure Password (length 8), SaltKey (length 8) and VIKey (length 16).");
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(message);
            byte[] cipherTextBytes;
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, Aes.Encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Encrypt<T>(this T entity) => entity.ToJson().Encrypt();
        public static string Decrypt(this string encryptedMessage)
        {
            if (!Aes.IsConfigured)
                throw new ArgumentException("Please use CryptingExtensions.Aes.Configure() method to configure Password (length 8), SaltKey (length 8) and VIKey (length 16).");
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedMessage);
            using var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, Aes.Decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        public static T Decrypt<T>(this string encryptedMessage) => encryptedMessage.Decrypt().FromJson<T>();
    }
}