using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BluClinicsApi.Helpers
{
    public class PasswordHelpers
    {
        private static readonly byte[] Pepper = Encoding.UTF8.GetBytes("!@BlueCloudB($$!@12");
        public static string HashPassword(string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Pepper,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));
            return hashed;
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            string hashed = HashPassword(password);
            return hashed == storedHash;
        }

    }
}