using System.Collections.Generic;
using WordsSearch.Core.Parsing.Interfaces;
using WordsSearch.Core.Utils;

namespace WordsSearch.Core.Parsing.Implementation
{
    public class PatternLoader : IPatternLoader
    {
        public List<string> Load(string pathToFile)
        {
            return TextFileHelper.ReadNotEmptyOrWhiteSpaceLines(pathToFile);
        }
    }
}