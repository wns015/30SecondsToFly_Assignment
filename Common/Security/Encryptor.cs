using System.Security.Cryptography;
using System.Text;

namespace Common.Security
{
    public class Encryptor
    {
       
        public static string EncryptText(string value)
        {
            var bytesSourceValue = Encoding.UTF8.GetBytes(value);
            var passwordBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Helper.SaltPassword));
            var randomSalt = Helper.GetSaltRandomBytes();

            // Add salt random value
            var bytesToBeEncryptedValue = new byte[randomSalt.Length + bytesSourceValue.Length];
            Array.Copy(randomSalt, bytesToBeEncryptedValue, randomSalt.Length);
            Array.Copy(bytesSourceValue, 0, bytesToBeEncryptedValue, randomSalt.Length, bytesSourceValue.Length);
            var bytesEncrypted = AesEncrypt(bytesToBeEncryptedValue, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted);
        }

        private static byte[] AesEncrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;
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
                    using (var cs = new CryptoStream(ms, creatorAes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                    creatorAes.Clear();
                }
                ms.Close();
            }
            return encryptedBytes;
        }

    }
}
