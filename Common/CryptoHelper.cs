using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 加密助手
    /// </summary>
    public static class CryptoHelper
    {
        /// <summary>
        /// 產生 SHA-256 加鹽（Salt）雜湊
        /// </summary>
        /// <param name="data">資料</param>
        /// <param name="salt">鹽</param>
        /// <returns>雜湊字串</returns>
        public static string GenerateHash(string data, string salt)
        {
            byte[] dataAndsaltBytes = System.Text.Encoding.UTF8.GetBytes(data + salt);
            using (SHA256Managed SHA256_Managed = new SHA256Managed())
            {
                byte[] hashBytes = SHA256_Managed.ComputeHash(dataAndsaltBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// 產生 HMACSHA256 雜湊
        /// </summary>
        /// <param name="data">資料</param>
        /// <param name="key">金鑰</param>
        /// <returns>雜湊字串</returns>
        public static string ComputeHMACSHA256(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (var HMACSHA256_Managed = new HMACSHA256(keyBytes))
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var hashBytes = HMACSHA256_Managed.ComputeHash(dataBytes, 0, dataBytes.Length);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="clearText">明文</param>
        /// <param name="key">私密金鑰</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="mode">作業模式</param>
        /// <param name="padding">填補模式</param>
        /// <param name="output">輸出(0:Base64/1:hex)</param>
        /// <param name="size">長度</param>
        /// <returns>密文</returns>
        public static string AESEncode(string clearText, string key, string iv, CipherMode mode, PaddingMode padding, int output, int? size)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = mode;
            rijndaelCipher.Padding = padding;

            if(size.HasValue)
            {
                rijndaelCipher.KeySize = size.Value;
                rijndaelCipher.BlockSize = size.Value;
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            rijndaelCipher.Key = keyBytes;
            if (!string.IsNullOrWhiteSpace(iv))
            {
                byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                rijndaelCipher.IV = ivBytes;
            }
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(clearText);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            string ciphertext = string.Empty;

            switch (output)
            {
                case 0:     //輸出爲Base64
                    ciphertext = Convert.ToBase64String(cipherBytes);
                    break;
                case 1:     //輸出爲hex
                    ciphertext = ToHex(cipherBytes);
                    break;
                default:
                    break;
            }

            return ciphertext;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">私密金鑰</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="mode">作業模式</param>
        /// <param name="padding">填補模式</param>
        /// <param name="output">輸出(0:Base64/1:hex)</param>
        /// <param name="size">長度</param>
        /// <returns>明文</returns>
        public static string AESDecode(string text, string key, string iv, CipherMode mode, PaddingMode padding, int output, int? size)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = mode;
            rijndaelCipher.Padding = padding;

            if(size.HasValue)
            {
                rijndaelCipher.KeySize = size.Value;
                rijndaelCipher.BlockSize = size.Value;
            }

            byte[] encryptedData;
            switch (output)
            {
                case 0:     //輸出爲Base64
                    encryptedData = Convert.FromBase64String(text);
                    break;
                case 1:     //輸出爲hex
                    encryptedData = UnHex(text);
                    break;
                default:
                    encryptedData = null;
                    break;
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            rijndaelCipher.Key = keyBytes;
            if (!string.IsNullOrWhiteSpace(iv))
            {
                byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
                rijndaelCipher.IV = ivBytes;
            }
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        #region Hex與byte轉碼
        /// <summary>
        /// 將 byte 數組 轉換成 hex
        /// </summary>
        /// <param name="bytes">需要轉碼的byte</param>
        /// <returns>hex</returns>
        private static string ToHex(byte[] bytes)
        {
            string str = string.Empty;
            if (bytes != null || bytes.Length > 0)
            {
                //將 byte 數組 轉換成 hex
                for (int i = 0; i < bytes.Length; i++)
                {
                    str += string.Format("{0:X2}", bytes[i]);
                }
            }
            return str.ToLower();
        }

        /// <summary>
        /// 將 hex 轉換成 byte 數組
        /// </summary>
        /// <param name="hex">需要轉碼的hex</param>
        /// <returns>byte 數組</returns>
        public static byte[] UnHex(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格
                throw new ArgumentException("hex is not a valid number!", "hex");
            }

            // 將 hex 轉換成 byte 數組。
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每兩個字符是一個 byte。
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            return bytes;
        }
        #endregion
    }
}
