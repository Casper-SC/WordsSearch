using System.Collections.Generic;

namespace WordsSearch.Core.Parsing.Interfaces
{
    public interface IFilesScanner
    {
        List<string> Scan(string source);
    }
}
