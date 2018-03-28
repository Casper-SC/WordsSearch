using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WordsSearch.Core.Parsing.Interfaces;

namespace WordsSearch.Core.Parsing.Implementation
{
    public class PatternParser : IParser
    {
        private readonly bool _ignoreCase;

        public PatternParser(bool ignoreCaseInRegExpressions)
        {
            _ignoreCase = ignoreCaseInRegExpressions;
        }

        public async Task<IReadOnlyList<CoincidenceContainer>> ParseAsync(string text, List<string> patternList)
        {
            var matches = new ConcurrentBag<CoincidenceContainer>();
            var patterns = new ConcurrentBag<string>(patternList);

            int taskCount = Environment.ProcessorCount;
            var tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    string pattern;
                    while (!patterns.IsEmpty)
                    {
                        if (patterns.TryTake(out pattern))
                        {
                            CoincidenceContainer container = FindAllOccurrences(pattern, text);

                            if (container != null)
                            {
                                matches.Add(container);
                            }
                        }
                    }
                });
            }

            await Task.WhenAll(tasks);

            return matches.ToArray();
        }

        private CoincidenceContainer FindAllOccurrences(string pattern, string text)
        {
            var options = _ignoreCase ? RegexOptions.Compiled | RegexOptions.IgnoreCase : RegexOptions.Compiled;
            var regex = new Regex(pattern, options);

            int lastStartIndex = 0;

            CoincidenceContainer container = null;
            while (true)
            {
                Match match = regex.Match(text, lastStartIndex);
                if (!match.Success)
                {
                    break;
                }

                lastStartIndex = match.Index + match.Length;

                if (container == null)
                {
                    container = new CoincidenceContainer();
                }

                string result = match.Value.Trim('\r', '\n');
                if (!container.Values.ContainsKey(result))
                {
                    // Одному шаблону могут соответствовать разные слова, в качестве ключа найденное слово.
                    container.Values.Add(result, new Coincidence(pattern, match.Length, result));
                }

                container.Values[result].Indexes.Add(match.Index);
            }

            return container;
        }
    }
}