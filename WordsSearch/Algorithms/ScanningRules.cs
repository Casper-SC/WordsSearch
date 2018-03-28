using System.Collections.Generic;

namespace WordsSearch.Algorithms
{
    public class ScanningRules
    {
        public ScanningRules(HashSet<string> excludedExtensions, 
            HashSet<string> excludedFolders, 
            HashSet<string> selectedExtensions,
            ScanStrategy strategy)
        {
            Strategy = strategy;
            ExcludedExtensions = excludedExtensions;
            ExcludedFolders = excludedFolders;
            SelectedExtensions = selectedExtensions;
        }

        public ScanStrategy Strategy { get; }

        /// <summary>
        /// Исключённые расширения файлов, которые должны быть проигнормрованы сканером файлов.
        /// </summary>
        public HashSet<string> ExcludedExtensions { get; }

        /// <summary>
        /// Выбранные расширения файлов, которые должен учитывать сканер файлов, но игнорировать все остальные.
        /// </summary>
        public HashSet<string> SelectedExtensions { get; }

        /// <summary>
        /// Исключённые папки, которые сканер файлов должен пропускать.
        /// </summary>
        public HashSet<string> ExcludedFolders { get; }

        /// <summary>
        /// Стратегия сканирования директорий на подходчящие файлы для парсинга.
        /// </summary>
        public enum ScanStrategy
        {
            /// <summary>
            /// Использовать все расширения.
            /// </summary>
            AllExtensions,

            /// <summary>
            /// Использовать все расширения, кроме исключённых.
            /// </summary>
            UseExcludedExtensions,

            /// <summary>
            /// Использовать расширения файлов, выбранные пользователем.
            /// </summary>
            UseSelectedExtensions
        }
    }
}