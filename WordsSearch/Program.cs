using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Autofac;
using NLog;
using WordsSearch.Algorithms;
using WordsSearch.Algorithms.Enums;
using WordsSearch.Algorithms.FileScanners;
using WordsSearch.Core.Parsing;
using WordsSearch.Core.Parsing.Implementation;
using WordsSearch.Core.Parsing.Interfaces;
using WordsSearch.Utils.Performance;
using WordsSearch.Utils.Performance.Formatting;
using WordsSearch.Utils.Performance.Formatting.Interfaces;

namespace WordsSearch
{
    class Program
    {
        private static IContainer Container { get; set; }

        #region Entry point

        private static Program _program;

        private static void Main(string[] args)
        {
            _program = new Program();
            _program.Run(args);
        }

        #endregion

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Запустить приложение.
        /// </summary>
        /// <param name="args">
        /// Аргумент 0:
        ///   "-fs"  = File scan.
        ///   "-ds"  = Directory scan.
        ///   "-dls" = Directory list scan.
        /// 
        /// Аргумент 1:
        ///   "Путь к файлу", который будет просканирован, если первый аргумент "-fs".
        ///   "Путь к папке", которая будет просканирована, если первый аргумент "-ds".
        ///   "Путь к файлу", из которого будет загружен список папок, которые будут 
        ///   просканированы (включая все вложенные), если первый аргумент "-dls".
        ///   Формат файла: путь к каждой корневой папке на новой строке.
        /// 
        /// Аргумент 2:
        ///   "Путь к файлу", содержащему список регулярных выражений.
        /// 
        /// Аргумент 3:
        ///   "Путь к файлу", в который будет записан результат.
        ///
        /// Пример:
        ///  "-dls" "С:\Parameters\DirectoryList.txt" "С:\Parameters\Patterns.txt" "С:\Parameters\ScanningResult.txt"
        /// </param>
        private void Run(string[] args)
        {
            if (args.Length < 3)
            {
                _logger.Error("Not enough positional command-line arguments specified!");
                Console.ReadKey();
                return;
            }

            string argLine = "\"" + string.Join("\" \"", args) + "\"";
            _logger.Debug("Command-line arguments: " + argLine);

            try
            {
                ScanStrategy scanStrategy = ParseScanStrategy(args[0]);

                string optionalParameter = args[1];
                string patternsFile = args[2];
                string targetFile = args[3];

                switch (scanStrategy)
                {
                    case ScanStrategy.File:
                        if (!CheckFile(optionalParameter)) return;
                        break;

                    case ScanStrategy.Directory:
                        if (!CheckDirectory(optionalParameter)) return;
                        break;

                    case ScanStrategy.Directories:
                        if (!CheckFile(optionalParameter)) return;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(scanStrategy));
                }

                if (!CheckFile(patternsFile)) return;

                Container = RegisterDependencies(scanStrategy, LoadScanningRules());

                var algoritm = Container.Resolve<FindWordsByPatternsAlgorithm>();

                algoritm.StartAsync(optionalParameter, patternsFile, targetFile)
                    .Wait();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            Console.ReadKey();
        }

        private ScanStrategy ParseScanStrategy(string key)
        {
            switch (key)
            {
                // file scan
                case "-fs":
                    return ScanStrategy.File;

                // directory scan
                case "-ds":
                    return ScanStrategy.Directory;

                // directory list scan
                case "-dls":
                    return ScanStrategy.Directories;

                default:
                    throw new ArgumentOutOfRangeException(nameof(key), "Scan strategy key is invalid.");
            }
        }

        private static IContainer RegisterDependencies(ScanStrategy scanStrategy, ScanningRules scanningRules)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<PatternParser>()
                .As<IParser>()
                .WithParameter("ignoreCaseInRegExpressions", true);

            builder.RegisterType<PatternLoader>()
                .As<IPatternLoader>();

            builder.RegisterType<FullResultFormatter>()
                .As<IResultFormatter>();

            builder.RegisterType<PerformanceMeasurement>()
                .As<IPerformanceMeasurement>();

            builder.RegisterType<ParsingResultByOneSourceFormatter>()
                .As<IFormatter<IReadOnlyList<CoincidenceContainer>, string>>();

            builder.RegisterType<ParsingResultByAllSourcesFormatter>()
                .As<IFormatter<Dictionary<string, List<CoincidenceResult>>, string>>();

            switch (scanStrategy)
            {
                case ScanStrategy.File:
                    builder.RegisterType<OneFileScanner>()
                        .As<IFilesScanner>();
                    break;

                case ScanStrategy.Directory:
                    builder.RegisterType<DirectoriesScanner>()
                        .As<IFilesScanner>();
                    break;

                case ScanStrategy.Directories:
                    builder.RegisterType<SelectedDirectoriesScanner>()
                        .As<IFilesScanner>();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(scanStrategy), scanStrategy, null);
            }

            builder.RegisterType<ScanningRules>()
                .WithParameter("strategy", scanningRules.Strategy)
                .WithParameter("selectedExtensions", scanningRules.SelectedExtensions)
                .WithParameter("excludedExtensions", scanningRules.ExcludedExtensions)
                .WithParameter("excludedFolders", scanningRules.ExcludedFolders)
                .AsSelf();

            builder.RegisterType<FindWordsByPatternsAlgorithm>()
                .AsSelf();

            return builder.Build();
        }

        private static bool CheckFile(string file)
        {
            if (!File.Exists(file))
            {
                _logger.Error("File \"{0}\" does not exist.", file);
                return false;
            }

            return true;
        }

        private static bool CheckDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                _logger.Error("Directory \"{0}\" does not exist.", directory);
                return false;
            }

            return true;
        }

        private static string[] GetSettingsAsStrings(string parameterName)
        {
            string value = ConfigurationManager.AppSettings[parameterName];
            return value.Split(new[] { ",", " ", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string GetSettingsAsString(string parameterName)
        {
            return ConfigurationManager.AppSettings[parameterName];
        }

        private static ScanningRules LoadScanningRules()
        {
            string value = GetSettingsAsString("ScanningRules.ScanStrategy");
            var scanStrategy = (ScanningRules.ScanStrategy)Enum.Parse(typeof(ScanningRules.ScanStrategy), value);
            return new ScanningRules(
                new HashSet<string>(GetSettingsAsStrings("ExcludedExtensions")),
                new HashSet<string>(GetSettingsAsStrings("ExcludedFolders")),
                new HashSet<string>(GetSettingsAsStrings("SelectedExtensions")),
                scanStrategy
                );
        }
    }
}
