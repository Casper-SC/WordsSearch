using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordsSearch.Core.Parsing.Interfaces;

namespace WordsSearch.Core.Parsing.Implementation
{
    public class ParsingResultByAllSourcesFormatter : IFormatter<Dictionary<string, List<CoincidenceResult>>, string>
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder(1536);

        public string Format(Dictionary<string, List<CoincidenceResult>> containers)
        {
            FormatHeader(_stringBuilder);
            FormatResult(_stringBuilder, containers);
            var temp = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return temp;
        }

        private void FormatHeader(StringBuilder sb)
        {
            const int Padding = 15;
            sb.AppendFormat("{0}{1}",
                "Word:".PadRight(Padding),
                Environment.NewLine);
            sb.AppendLine(new string('-', Padding * 2));
        }

        private void FormatResult(StringBuilder sb, Dictionary<string, List<CoincidenceResult>> containerDictionary)
        {
            var uniqueWords = new Dictionary<string, HashSet<string>>();

            // Key - MD5 хэш файла, Value - Список результатов парсинга.
            foreach (KeyValuePair<string, List<CoincidenceResult>> resPair in containerDictionary)
            {
                foreach (CoincidenceResult result in resPair.Value)
                {
                    foreach (CoincidenceContainer container in result.Containers)
                    {
                        foreach (KeyValuePair<string, Coincidence> pair in container.Values)
                        {
                            if (!uniqueWords.ContainsKey(pair.Value.Match))
                            {
                                uniqueWords.Add(pair.Value.Match, new HashSet<string>());
                            }
                            uniqueWords[pair.Value.Match].Add(result.FileExtension);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, HashSet<string>> pair in uniqueWords)
            {
                string word = pair.Key;
                string extensions = pair.Value.Aggregate((agg, next) => agg + ", " + next);
                sb.AppendLine($"{word} ({extensions})");
            }
        }
    }
}