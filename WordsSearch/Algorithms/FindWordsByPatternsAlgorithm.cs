using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NLog;
using WordsSearch.Core.Parsing;
using WordsSearch.Core.Parsing.Interfaces;
using WordsSearch.Utils.Performance;
using WordsSearch.Utils.Utils;

namespace WordsSearch.Algorithms
{
    public class FindWordsByPatternsAlgorithm
    {
        private readonly IParser _parser;
        private readonly IPatternLoader _patternLoader;
        private readonly IPerformanceMeasurement _performance;
        private readonly IFormatter<IReadOnlyList<CoincidenceContainer>, string> _formatterByOneFile;
        private readonly IFormatter<Dictionary<string, List<CoincidenceResult>>, string> _formatterByAllFiles;

        private readonly IFilesScanner _filesScanner;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public FindWordsByPatternsAlgorithm(IParser parser,
            IPatternLoader patternLoader,
            IPerformanceMeasurement performance,
            IFormatter<IReadOnlyList<CoincidenceContainer>, string> formatterByOneFile,
            IFormatter<Dictionary<string, List<CoincidenceResult>>, string> formatterByAllFiles,
            IFilesScanner filesScanner)
        {
            _parser = parser;
            _patternLoader = patternLoader;
            _performance = performance;
            _formatterByOneFile = formatterByOneFile;
            _formatterByAllFiles = formatterByAllFiles;
            _filesScanner = filesScanner;

            _performance.LogStartNew += Performance_LogStartNew;
            _performance.LogEnd += Performance_LogEnd;
        }

        public async Task StartAsync(string source, string patternsFile, string outputFile)
        {
            _logger.Info("Asynchronous parsing is started...");

            List<string> patterns = LoadPatterns(patternsFile);
            _logger.Info("");

            var fileList = _filesScanner.Scan(source);

            _logger.Info("The number of files to check: " + fileList.Count);

            int digitsCount = MathUtils.GetDigitsCount(fileList.Count);
            string StartedParsingOfFileTemplate = "{1:D" + digitsCount + "}" + "/" + "{2:D" + digitsCount + "}: Started parsing of \"{0}\"";
            string SkippedParsingOfFileTemplate = "{1:D" + digitsCount + "}" + "/" + "{2:D" + digitsCount + "}: Skipped parsing of \"{0}\" (Duplicate)";

            var stringBuilder = new StringBuilder();
            // Key - MD5 хэш файла, Value - Список результатов парсинга.
            var containers = new Dictionary<string, List<CoincidenceResult>>(fileList.Count);

            for (int index = 0; index < fileList.Count; index++)
            {
                string pathToFile = fileList[index];
                string checksum = Hash.GetMD5HashOfFile(pathToFile);
                string directory = Path.GetDirectoryName(pathToFile);
                string fileName = Path.GetFileName(pathToFile);

                _logger.Info($"Directory \"{directory}\"");

                string text = LoadText(pathToFile, fileName);
                _logger.Info("");

                // Содержит разные найденные по шаблону слова для одного файла
                IReadOnlyList<CoincidenceContainer> result = null;
                // Если файл с таким хэшем ещё не парсили (в разных папках могут быть одинаковые файлы)
                if (!containers.ContainsKey(checksum))
                {
                    try
                    {
                        _performance.StartNew(string.Format(StartedParsingOfFileTemplate, fileName, index + 1, fileList.Count));
                        result = await _parser.ParseAsync(text, patterns);
                    }
                    finally
                    {
                        _performance.End("Took time");
                    }

                    if (result.Count != 0)
                    {
                        containers.Add(checksum, new List<CoincidenceResult>
                        {
                            new CoincidenceResult(fileName, checksum, result)
                        });
                    }
                }
                else
                {
                    if (containers[checksum].Count > 0)
                    {
                        _logger.Info(SkippedParsingOfFileTemplate, fileName, index + 1, fileList.Count);

                        result = containers[checksum][0].Containers;
                        containers[checksum].Add(new CoincidenceResult(fileName, checksum, result));
                    }
                }

                _logger.Info("");
                if (result != null && result.Count != 0)
                {
                    string formattedText = _formatterByOneFile.Format(result);

                    stringBuilder.AppendLine($"File: \"{pathToFile}\"");
                    stringBuilder.Append(formattedText);
                    stringBuilder.AppendLine();

                    _logger.Info(formattedText);
                }
                else
                {
                    const string NotFound = "Words not found.";
                    _logger.Info(NotFound);
                }

                _logger.Info("--------------" + Environment.NewLine);
            }

            string wordsFoundText = _formatterByAllFiles.Format(containers);
            _logger.Info(wordsFoundText);
            stringBuilder.AppendLine(wordsFoundText);

            _logger.Info($"Writing result to file \"{outputFile}\"...");
            WriteResult(outputFile, stringBuilder.ToString());
            _logger.Info("Parsing completed.");
        }

        private string LoadText(string sourceTextFile, string fileName)
        {
            string text = null;
            _performance.Measure(() =>
            {
                text = File.ReadAllText(sourceTextFile);
            },
            $"Loading of file \"{fileName}\"",
            "Took time");
            return text;
        }

        private List<string> LoadPatterns(string file)
        {
            List<string> patterns = null;
            _performance.Measure(() =>
            {
                patterns = _patternLoader.Load(file);
            },
            "Loading of patterns...",
            "Took time");
            return patterns;
        }

        private void Performance_LogStartNew(IPerformanceMeasurement sender, string message)
        {
            _logger.Info(message);
        }

        private void Performance_LogEnd(IPerformanceMeasurement sender, string message)
        {
            _logger.Debug(message);
        }

        private void WriteResult(string fileName, string text)
        {
            using (var sw = File.CreateText(fileName))
            {
                sw.Write(text);
            }
        }
    }
}
