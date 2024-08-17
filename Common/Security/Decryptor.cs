using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Security
{
    public class Decryptor
    {
        public static string DecryptText(string value)
        {
            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(value);
            var passwordBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Helper.SaltPassword));
            var bytesDecrypted = AesDecrypt(bytesToBeDecrypted, passwordBytes);
            var randomSaltLength = Helper.SaltLength;

            // Remove salt random value
            var decryptedResult = new byte[bytesDecrypted.Length - randomSaltLength];
            Array.Copy(bytesDecrypted, randomSaltLength, decryptedResult, 0, decryptedResult.Length);
            return Encoding.UTF8.GetString(decryptedResult);
        }

        private static byte[] AesDecrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes;
            var saltBytes = Encoding.UTF8.GetBytes(Helper.SaltPassword);
            using (var ms = new MemoryStream())
            {
                using (var creatorAes = new AesCryptoServiceProvider())
                {
                    creatorAes.KeySize = 256;
                    creatorAes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    creatorAes.Key = key.GetBytes(creatorAes.KeySize / 8);
                    creatorAes.IV = key.GetBytes(creatorAes.BlockSize / 8);

                    creatorAes.Mode = CipherMode.CBC;
                    creatorAes.Padding = PaddingMode.PKCS7;

                    using (var cs = new CryptoStream(ms, creatorAes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                    creatorAes.Clear();
                }
                ms.Close();
            }

            return decryptedBytes;
        }

    }
}