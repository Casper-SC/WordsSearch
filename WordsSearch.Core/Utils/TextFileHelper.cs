using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordsSearch.Core.Utils
{
    public static class TextFileHelper
    {
        public static List<string> ReadNotEmptyOrWhiteSpaceLines(string pathToFile, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var lines = new List<string>(300);
            foreach (var line in File.ReadLines(pathToFile, encoding))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line.Trim());
                }
            }

            return lines;
        }
    }
}
