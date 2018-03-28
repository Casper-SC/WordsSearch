using System.Collections.Generic;
using System.IO;
using WordsSearch.Core.Parsing.Interfaces;

namespace WordsSearch.Algorithms.FileScanners
{
    public class OneFileScanner : IFilesScanner
    {
        public List<string> Scan(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException("File not found", pathToFile);
            }

            return new List<string>(1) { pathToFile };
        }
    }
}
