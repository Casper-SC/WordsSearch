using System.Collections.Generic;
using System.Diagnostics;

namespace WordsSearch.Core.Parsing
{
    /// <summary>
    /// Точное совпадение.
    /// </summary>
    [DebuggerDisplay("Indexes.Count = {Indexes.Count}, Match = {Match}, Pattern = {Pattern}")]
    public class Coincidence
    {
        public Coincidence(string pattern, int length, string match, int indexesCapacity = 4)
        {
            Indexes = new List<int>(indexesCapacity);
            Length = length;
            Match = match;
            Pattern = pattern;
        }

        /// <summary>
        /// Шаблон, по которому ищется слово в тексте.
        /// </summary>
        public string Pattern { get; private set; }

        /// <summary>
        /// Индексы, по которым можно определить позиции одного и того же слова, 
        /// встречающегося в разных местах в тексте.
        /// </summary>
        public List<int> Indexes { get; private set; }

        /// <summary>
        /// Длина слова.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Слово, найденное по шаблону.
        /// </summary>
        public string Match { get; private set; }

    }
}