using System.Collections.Generic;
using System.Diagnostics;

namespace WordsSearch.Core.Parsing
{
    /// <summary>
    /// Контейнер точных совпадений. Предназначается для группировки совпадений, соответствующих одному шаблону.
    /// Каждый контейнер содержит совпадения соответствующие одному шаблону 
    /// (шаблон один, слова разные, каждое слово может встречаться несколько раз в разных местах по тексту).
    /// </summary>
    [DebuggerDisplay("Values.Count = {Values.Count}")]
    public class CoincidenceContainer
    {
        public CoincidenceContainer(int capacity = 4)
        {
            Values = new SortedList<string, Coincidence>(capacity);
        }
        
        /// <summary>
        /// Key = Результат поиска, Value = Точное совпадение.
        /// </summary>
        public SortedList<string, Coincidence> Values { get; private set; }
    }
}