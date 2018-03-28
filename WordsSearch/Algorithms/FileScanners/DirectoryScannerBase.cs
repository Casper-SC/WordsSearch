using System;
using System.Collections.Generic;
using System.IO;

namespace WordsSearch.Algorithms.FileScanners
{
    public abstract class DirectoryScannerBase
    {
        protected void DirectoryScan(string root, List<string> fileList, ScanningRules scanRules, bool checkExcludedFolders)
        {
            foreach (string file in Directory.EnumerateFiles(root))
            {
                switch (scanRules.Strategy)
                {
                    case ScanningRules.ScanStrategy.AllExtensions:
                        fileList.Add(file);
                        break;

                    case ScanningRules.ScanStrategy.UseExcludedExtensions:
                        if (!scanRules.ExcludedExtensions.Contains(Path.GetExtension(file)))
                        {
                            fileList.Add(file);
                        }
                        break;

                    case ScanningRules.ScanStrategy.UseSelectedExtensions:
                        if (scanRules.SelectedExtensions.Contains(Path.GetExtension(file)))
                        {
                            fileList.Add(file);
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(scanRules.Strategy),
                            $"{nameof(scanRules)}.{nameof(scanRules.Strategy)} == {scanRules.Strategy.ToString()}");
                }
            }

            foreach (string directory in Directory.EnumerateDirectories(root))
            {
                if (checkExcludedFolders)
                {
                    string currentFolderName = Path.GetFileName(directory);
                    if (scanRules.ExcludedFolders.Contains(currentFolderName))
                    {
                        continue;
                    }
                }

                DirectoryScan(directory, fileList, scanRules, false);
            }
        }
    }
}