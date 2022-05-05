using System;
using System.Security.Cryptography;

namespace Rystem
{
    public static class Alea
    {
        public static string GetTimedKey()
            => string.Format("{0:d19}{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, Guid.NewGuid().ToString("N"));
        public static string GetTimedKey(DateTime date)
            => string.Format("{0:d19}{1}", DateTime.MaxValue.Ticks - date.Ticks, Guid.NewGuid().ToString("N"));
        public static int GetNumber(int max)
        {
            int maxPlusOne = max + 1;
            return RandomNumberGenerator.GetInt32(maxPlusOne);
        }
    }
}