using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Opine.Security
{
    /// <summary>
    /// Hashing helper class.
    /// https://tahirnaushad.com/2017/09/09/hashing-in-asp-net-core-2-0/
    /// </summary>
    public class Hasher
    {
        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public static string CreateHash(string value)
        {
            var salt = CreateSalt();
            var hash = CreateHash(value, salt);
            return salt + ":" + hash;
        }

        public static string CreateHash(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);
 
            var hash = Convert.ToBase64String(valueBytes);
            return hash;
        }

        public static bool Validate(string value, string hash)
        {
            var index = hash.IndexOf(":");
            var salt = hash.Substring(0, index);
            var hashed = hash.Substring(index + 1);
            return Validate(value, salt, hashed);
        }
 
        public static bool Validate(string value, string salt, string hash)
        {
            var newHash = CreateHash(value, salt);
            return newHash == hash;
        } 
    }
}