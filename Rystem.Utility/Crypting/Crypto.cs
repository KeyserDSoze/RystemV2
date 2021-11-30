using Rystem.IO;
using Rystem.Text;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Security.Cryptography
{
    public static class Crypto
    {
        public static class Aes
        {
            public static void Configure(AesCryptoOptions options)
            {
                Password = options.Password;
                SaltKeyAsBytes = Encoding.ASCII.GetBytes(options.SaltKey);
                VIKeyAsBytes = Encoding.ASCII.GetBytes(options.VIKey);
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
        public static async Task<string> EncryptAsync(this string message)
        {
            if (!Aes.IsConfigured)
                throw new ArgumentException("Please use Crypto.Aes.Configure() method to configure Password (length 8), SaltKey (length 8) and VIKey (length 16).");
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(message);
            byte[] cipherTextBytes;
            using (MemoryStream memoryStream = new())
            {
                using var cryptoStream = new CryptoStream(memoryStream, Aes.Encryptor, CryptoStreamMode.Write);
                await cryptoStream.WriteAsync(plainTextBytes, 0, plainTextBytes.Length).NoContext();
                await cryptoStream.FlushFinalBlockAsync().NoContext();
                memoryStream.Position = 0;
                cipherTextBytes = memoryStream.ToArray();
            }
            var val = Convert.ToBase64String(cipherTextBytes);
            return val;
        }
        public static Task<string> EncryptAsync<T>(this T entity) => entity.ToJson().EncryptAsync();
        public static string Encrypt<T>(this T entity) => entity.EncryptAsync().ToResult();
        public static string Encrypt(this string message) => message.EncryptAsync().ToResult();
        public static async Task<string> DecryptAsync(this string encryptedMessage)
        {
            if (!Aes.IsConfigured)
                throw new ArgumentException("Please use CryptingExtensions.Aes.Configure() method to configure Password (length 8), SaltKey (length 8) and VIKey (length 16).");
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedMessage);
            Memory<byte> buffer = new(new byte[cipherTextBytes.Length], 0, cipherTextBytes.Length);
            using (MemoryStream memoryStream = new(cipherTextBytes))
            {
                using CryptoStream cryptoStream = new(memoryStream, Aes.Decryptor, CryptoStreamMode.Read);
                int total = 0;
                int index = 0;
                do
                {
                    total += index = await cryptoStream.ReadAsync(buffer.Slice(total)).NoContext();
                } while (index > 0);
            }
            string val = buffer.ConvertToString().TrimEnd('\0');
            return val;
        }
        public static async Task<T> DecryptAsync<T>(this string encryptedMessage) => (await encryptedMessage.DecryptAsync().NoContext()).FromJson<T>();
        public static T Decrypt<T>(this string encryptedMessage) => encryptedMessage.DecryptAsync().ToResult().FromJson<T>();
        public static string Decrypt(this string encryptedMessage) => encryptedMessage.DecryptAsync().ToResult();
    }
}