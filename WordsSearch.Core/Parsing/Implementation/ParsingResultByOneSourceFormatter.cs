using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordsSearch.Core.Parsing.Interfaces;
//using System.Threading;

namespace WordsSearch.Core.Parsing.Implementation
{
    public class ParsingResultByOneSourceFormatter : IFormatter<IReadOnlyList<CoincidenceContainer>, string>
    {
        //private static readonly ThreadLocal<StringBuilder> _stringBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder(4096));
        private readonly StringBuilder _stringBuilder = new StringBuilder(4096);

        public string Format(IReadOnlyList<CoincidenceContainer> containers)
        {
            //var sb = _stringBuilder.Value;
            var sb = _stringBuilder;
            FormatHeader(sb);
            FormatMatches(sb, containers, CoincidenceSort.ByMatch);
            var temp = sb.ToString();
            sb.Clear();
            return temp;
        }

        private void FormatMatches(StringBuilder sb, IReadOnlyList<CoincidenceContainer> containers, CoincidenceSort sortingRule)
        {
            var result = new SortedList<string, Coincidence>(containers.Sum(c => c.Values.Count));

            foreach (var container in containers)
            {
                foreach (KeyValuePair<string, Coincidence> pair in container.Values)
                {
                    string key = sortingRule == CoincidenceSort.ByMatch
                        // Каждое найденное слово уникально
                        ? pair.Value.Match
                        // Одному паттерну могут соответствовать несколько разных слов
                        : pair.Value.Pattern + pair.Value.Match;

                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, pair.Value);
                    }
                }
            }

            foreach (KeyValuePair<string, Coincidence> pair in result)
            {
                FormatResult(sb, pair.Value);

                const int Padding = 15;
                sb.AppendLine(new string('-', Padding * 5));
            }
        }

        private void FormatHeader(StringBuilder sb)
        {
            const int Padding = 15;
            sb.AppendFormat("## {0}{1}{2}{3}{4}",
                "Index:".PadRight(Padding),
                "Length:".PadRight(Padding),
                "Value:".PadRight(Padding),
                "Stopword:".PadRight(Padding),
                Environment.NewLine);
            sb.AppendLine(new string('-', Padding * 5));
        }

        private void FormatResult(StringBuilder sb, Coincidence coincidence)
        {
            const int Padding = 15;
            for (int i = 0; i < coincidence.Indexes.Count; i++)
            {
                int index = coincidence.Indexes[i];
                if (i == 0)
                {
                    sb.AppendLine(string.Format("## {0}{1}{2}{3}",
                        index.ToString().PadRight(Padding),
                        coincidence.Length.ToString().PadRight(Padding),
                        coincidence.Match.PadRight(Padding),
                        coincidence.Pattern.PadRight(Padding))
                    );
                }
                else
                {
                    sb.AppendLine(string.Format("{0}{1}",
                        string.Empty.PadRight(3),
                        index.ToString().PadRight(Padding)));
                }
            }
        }

        private enum CoincidenceSort
        {
            /// <summary>
            /// Сортировать по найденным по шаблону словам.
            /// </summary>
            ByMatch,

            /// <summary>
            /// Сортировать по шаблону.
            /// </summary>
            ByPattern
        }
    }
}