using System.Collections.Generic;
using System.Threading.Tasks;

namespace WordsSearch.Core.Parsing.Interfaces
{
    public interface IParser
    {
        Task<IReadOnlyList<CoincidenceContainer>> ParseAsync(string text, List<string> patternList);
    }
}