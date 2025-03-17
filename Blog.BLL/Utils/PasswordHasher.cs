using System;
using System.Security.Cryptography;
using System.Text;

namespace BlogSystem.BLL.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            using var hmac = new HMACSHA512();
            passwordSalt = Convert.ToBase64String(hmac.Key);
            passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));
            if (string.IsNullOrWhiteSpace(storedHash))
                throw new ArgumentException("Invalid stored hash", nameof(storedHash));
            if (string.IsNullOrWhiteSpace(storedSalt))
                throw new ArgumentException("Invalid stored salt", nameof(storedSalt));

            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != hashBytes[i])
                    return false;
            }

            return true;
        }
    }

    public interface IPasswordHasher
    {
        void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt);
        bool VerifyPasswordHash(string password, string storedHash, string storedSalt);
    }
}