namespace CodingCode.Web.Logic
{
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contracts;

    public class CodeFounder : ICodeFounder
    {
        private readonly int _numKeywords;
        private readonly string[] _keywords;
        private readonly string _searchedPath;
        private readonly string _fileExtensionPattern;

        public CodeFounder(int numKeywords, string[] keywords, string searchedPath, string fileExtensionPattern)
        {
            _numKeywords = numKeywords;
            _keywords = keywords;
            _searchedPath = searchedPath;
            _fileExtensionPattern = fileExtensionPattern;
        }

        public string[] GetMachingFiles()
        {
            var fileList = FindMachingFiles();
            var numMachingFiles = CountMatchingfiles(fileList);
            return SortListOfFiles(numMachingFiles, fileList);
        }

        private string[] SortListOfFiles(int numMachingFiles,
            ConcurrentBag<string>[] fileList)
        {
            var result = new string[numMachingFiles];
            var nextFile = 0;
            for(var i = _numKeywords - 1; i >= 0; i--)
            {
                foreach(var file in fileList[i])
                {
                    result[nextFile++] = file;
                }
            }
            return result;
        }

        private int CountMatchingfiles(
            ConcurrentBag<string>[] fileList)
        {
            var numMachingFiles = 0;
            for(var i = 0; i < _numKeywords; i++)
            {
                numMachingFiles += fileList[i].Count();
            }
            return numMachingFiles;
        }

        private ConcurrentBag<string>[] FindMachingFiles()
        {
            var fileList = InitializeConcurrentBag(_numKeywords);
            var files = GetFiles();
            Parallel.ForEach(files, file =>
            {
                var text = File.ReadAllText(file.FullName);
                var count = -1 +
                            _keywords.Count(
                                keyword =>
                                    Regex.IsMatch(text, keyword,
                                        RegexOptions.IgnoreCase));
                if(count > -1)
                {
                    fileList[count].Add(file.FullName);
                }
            });
            return fileList;
        }

        private static ConcurrentBag<string>[] InitializeConcurrentBag(
            int numKeywords)
        {
            var fileList = new ConcurrentBag<string>[numKeywords];
            for(var i = 0; i < numKeywords; i++)
            {
                fileList[i] = new ConcurrentBag<string>();
            }
            return fileList;
        }


        private FileInfo[] GetFiles()
        {
            return
                new DirectoryInfo(_searchedPath).GetFiles(
                    _fileExtensionPattern,
                    SearchOption.AllDirectories);
        }
    }
}