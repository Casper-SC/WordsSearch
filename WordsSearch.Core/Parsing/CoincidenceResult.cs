using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WordsSearch.Core.Parsing
{
    /// <summary>
    /// Все совпадения в рамках одного файла.
    /// </summary>
    [DebuggerDisplay("FileName = {FileName}, FileChecksum = {FileChecksum}, Containers.Count = {Containers.Count}")]
    public class CoincidenceResult
    {
        public CoincidenceResult(string fileName, string fileChecksum, IReadOnlyList<CoincidenceContainer> containers)
        {
            FileName = fileName;
            FileChecksum = fileChecksum;
            Containers = containers;
        }

        /// <summary>
        /// Список контейнеров точных совпадений. 
        /// Один контейнер содержит шаблон и список совпадений, найденных по этому шаблону.
        /// </summary>
        public IReadOnlyList<CoincidenceContainer> Containers { get; set; }

        /// <summary>
        /// Контрольная сумма файла.
        /// </summary>
        public string FileChecksum { get; set; }

        /// <summary>
        /// Имя файла, в котором производился поиск по шаблонам.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Расширение файла, включающее точку.
        /// </summary>
        public string FileExtension
        {
            get { return Path.GetExtension(FileName); }
        }
    }
}
