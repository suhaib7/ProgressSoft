using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    using System.Security.Cryptography;
    using System.Text;

    public static class PasswordUtils
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash); // .NET 5+ only; use BitConverter.ToString if older
            }
        }

        public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var providedHash = HashPassword(providedPassword);
            return string.Equals(hashedPassword, providedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
