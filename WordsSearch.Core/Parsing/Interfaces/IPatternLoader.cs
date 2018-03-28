using System.Collections.Generic;

namespace WordsSearch.Core.Parsing.Interfaces
{
    public interface IPatternLoader
    {
        List<string> Load(string source);
    }
}