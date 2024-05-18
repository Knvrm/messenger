using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Protocol
{
    internal class Hash
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                // Combine the password and the salt
                var saltedPassword = Encoding.UTF8.GetBytes(password);
                var saltedPasswordWithSalt = new byte[saltedPassword.Length + salt.Length];

                Buffer.BlockCopy(saltedPassword, 0, saltedPasswordWithSalt, 0, saltedPassword.Length);
                Buffer.BlockCopy(salt, 0, saltedPasswordWithSalt, saltedPassword.Length, salt.Length);

                // Hash the salted password
                return sha256.ComputeHash(saltedPasswordWithSalt);
            }
        }
        public static bool AreHashesEqual(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
            {
                return false;
            }

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}

