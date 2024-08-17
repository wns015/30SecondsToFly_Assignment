using System.Security.Cryptography;

namespace Common.Security
{
    internal class Helper
    {
        internal static readonly string SaltPassword = "ke'o@not";

        internal static readonly int SaltLength = 8;

        internal static byte[] GetSaltRandomBytes()
        {
            var saltRandomByte = new byte[SaltLength];
            RandomNumberGenerator.Create().GetBytes(saltRandomByte);
            return saltRandomByte;
        }
    }
}
