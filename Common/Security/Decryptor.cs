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
        private static string IV = "2LsQwR6zzuY4GsET";
        private static string Key = "URunphkQDsiY47gLEXpHmNd9jKnzGTVH";

        public static string DecryptText(string encryptedString)
        {
            var bytes = StringToByteArray(encryptedString);
            var decryptedBytes = DecryptAes256HexByKey(bytes, Key, IV);

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static byte[] DecryptAes256HexByKey(byte[] bytesToBeDecrypted, string key, string iv)
        {
            byte[] decryptedBytes;
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