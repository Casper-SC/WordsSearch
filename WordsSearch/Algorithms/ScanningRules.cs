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
        /// ����������� ���������� ������, ������� ������ ���� ��������������� �������� ������.
        /// </summary>
        public HashSet<string> ExcludedExtensions { get; }

        /// <summary>
        /// ��������� ���������� ������, ������� ������ ��������� ������ ������, �� ������������ ��� ���������.
        /// </summary>
        public HashSet<string> SelectedExtensions { get; }

        /// <summary>
        /// ����������� �����, ������� ������ ������ ������ ����������.
        /// </summary>
        public HashSet<string> ExcludedFolders { get; }

        /// <summary>
        /// ��������� ������������ ���������� �� ����������� ����� ��� ��������.
        /// </summary>
        public enum ScanStrategy
        {
            /// <summary>
            /// ������������ ��� ����������.
            /// </summary>
            AllExtensions,

            /// <summary>
            /// ������������ ��� ����������, ����� �����������.
            /// </summary>
            UseExcludedExtensions,

            /// <summary>
            /// ������������ ���������� ������, ��������� �������������.
            /// </summary>
            UseSelectedExtensions
        }
    }
}