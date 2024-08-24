using System.Security.Cryptography;
using System.Text;

namespace Common.Security
{
    public class Encryptor
    {
        private static string IV = "2LsQwR6zzuY4GsET";
        private static string Key = "URunphkQDsiY47gLEXpHmNd9jKnzGTVH";
        public static string EncryptText(string value)
        {
            var bytesSourceValue = Encoding.UTF8.GetBytes(value);
            var bytesEncrypted = EncryptAesHexByKey(bytesSourceValue, Key, IV);
            return Convert.ToBase64String(bytesEncrypted);
        }

        private static byte[] EncryptAesHexByKey(byte[] bytesToBeEncrypted, string key, string iv)
        {
            byte[] encryptedBytes;
            using (var ms = new MemoryStream())
            {
                using (var creatorAes = new AesCryptoServiceProvider())
                {
                    creatorAes.KeySize = 256;
                    creatorAes.BlockSize = 128;
                    creatorAes.Key = Encoding.UTF8.GetBytes(key);
                    creatorAes.IV = Encoding.UTF8.GetBytes(iv);
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

        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

    }
}
