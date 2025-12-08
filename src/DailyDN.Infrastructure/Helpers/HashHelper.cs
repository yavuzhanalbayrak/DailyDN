using System;
using System.Security.Cryptography;
using System.Text;

namespace DailyDN.Infrastructure.Helpers
{
    public static class HashHelper
    {
        /// <summary>
        /// Verilen string'i SHA256 ile hashler ve Base64 olarak döner.
        /// </summary>
        public static string HashSha256(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verilen string'i SHA512 ile hashler ve Base64 olarak döner.
        /// </summary>
        public static string HashSha512(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));

            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA512.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Rastgele byte dizisi üretir ve Base64 string olarak döner.
        /// </summary>
        public static string GenerateRandomToken(int length = 32)
        {
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
