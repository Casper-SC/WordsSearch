using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WordsSearch.Utils.Utils
{
    /// <summary>
    /// Предоставляет методы хэширования.
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// Получить MD5 хэш строки
        /// </summary>
        /// <param name="textToHash">Строка, из которой будет получен хэш</param>
        /// <returns></returns>
        public static string GetMD5Hash(string textToHash)
        {
            MD5 mD = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(textToHash);
            byte[] value = mD.ComputeHash(bytes);
            return BitConverter.ToString(value).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// Получить MD5 хэш файла
        /// </summary>
        /// <param name="path">Файл, из которого будет получен хэш</param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        public static string GetMD5HashOfFile(string path, bool upperCase = true)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] checkSum = md5.ComputeHash(buffer);

                string hash = BitConverter.ToString(checkSum).Replace("-", string.Empty);
                return upperCase ? hash : hash.ToLowerInvariant();
            }
        }
    }
}