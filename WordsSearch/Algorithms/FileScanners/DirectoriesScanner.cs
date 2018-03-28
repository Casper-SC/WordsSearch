using System.Collections.Generic;
using System.IO;
using NLog;
using WordsSearch.Core.Parsing.Interfaces;
using WordsSearch.Utils.Performance;

namespace WordsSearch.Algorithms.FileScanners
{
    public class DirectoriesScanner : DirectoryScannerBase, IFilesScanner
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPerformanceMeasurement _performance;
        private readonly ScanningRules _scanningRules;

        public DirectoriesScanner(IPerformanceMeasurement performance, ScanningRules scanningRules)
        {
            _performance = performance;
            _scanningRules = scanningRules;

            _performance.LogStartNew += Performance_LogStartNew;
            _performance.LogEnd += Performance_LogEnd;
        }

        public List<string> Scan(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
            {
                throw new DirectoryNotFoundException("Directory not found. Directory: " + rootDirectory);
            }

            var fileList = new List<string>(2048);

            DirectoryScan(rootDirectory, fileList, _scanningRules, true);

            return fileList;
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