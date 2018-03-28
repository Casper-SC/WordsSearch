using System.Collections.Generic;
using System.IO;
using NLog;
using WordsSearch.Core.Parsing.Interfaces;
using WordsSearch.Core.Utils;
using WordsSearch.Utils.Performance;

namespace WordsSearch.Algorithms.FileScanners
{
    public class SelectedDirectoriesScanner : DirectoryScannerBase, IFilesScanner
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPerformanceMeasurement _performance;
        private readonly ScanningRules _scanningRules;

        public SelectedDirectoriesScanner(IPerformanceMeasurement performance, ScanningRules scanningRules)
        {
            _performance = performance;
            _scanningRules = scanningRules;

            _performance.LogStartNew += Performance_LogStartNew;
            _performance.LogEnd += Performance_LogEnd;
        }

        public List<string> Scan(string directoryListFile)
        {
            if (!File.Exists(directoryListFile))
            {
                throw new FileNotFoundException("File not found", directoryListFile);
            }

            var fileList = new List<string>(2048);
            List<string> sourceFolders = LoadDirectories(directoryListFile);
            foreach (string folder in sourceFolders)
            {
                DirectoryScan(folder, fileList, _scanningRules, true);
            }

            return fileList;
        }

        private List<string> LoadDirectories(string file)
        {
            List<string> patterns = null;
            _performance.Measure(() =>
            {
                patterns = TextFileHelper.ReadNotEmptyOrWhiteSpaceLines(file);
            }, "Loading of directories...", "Took time");
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
    }
}
