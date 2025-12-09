using System.Security.Cryptography;
using System.Text;

namespace Dr.meow.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); // 例如：AB12CD...
        }

        public static bool VerifyPassword(string password, string hashFromDb)
        {
            var hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hashFromDb, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
