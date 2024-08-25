
using Konscious.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Security
{
    public class PasswordUtil
    {
        private static readonly int Length = 16;

        public static string ArgonHashString(string password, string argonSalt)
        {
            var mixedSalt = AddSalt(password, argonSalt);
            byte[] mixedSaltBytes = Encoding.UTF8.GetBytes(mixedSalt);

            var hash = new Argon2id(mixedSaltBytes);

            hash.Iterations = 4; // The number of iterations to apply to the password hash
            hash.MemorySize = 65536; //The number of 1kB memory blocks to use while processing the hash (64 MB (256*256))
            hash.DegreeOfParallelism = 4; //The number of lanes to use while processing the hash

            return Convert.ToBase64String(hash.GetBytes(128)).Replace("\0", "");
        }

        public static bool ArgonHashStringVerify(string hash, string password, string argonSalt)
        {
            return hash.SequenceEqual(ArgonHashString(password, argonSalt));
        }

        private static string AddSalt(string password, string argonSalt)
        {
            return Helper.SaltPassword + password + argonSalt;
        }

        public static string ArgonSalt()
        {
            byte[] array = new byte[Length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(array);
                return Convert.ToBase64String(array);
            }
        }


    }
}