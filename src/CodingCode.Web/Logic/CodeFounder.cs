namespace CodingCode.Web.Logic
{
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contracts;
    using ViewModels;

    public class CodeFounder : ICodeFounder
    {
        public string[] GetMachingFiles(
            SearchedCodeViewModel searchedCode)
        {
            var keywords = searchedCode.Keywords.Split(',');
            var numKeywords = keywords.Length;
            var fileList = InitializeConcurrentBag(numKeywords);

            var files = GetFiles(searchedCode);
            var numMachingFiles = 0;
            Parallel.ForEach(files, file =>
            {
                var text = File.ReadAllText(file.FullName);
                var count = -1 +
                            keywords.Count(
                                keyword =>
                                    Regex.IsMatch(text, keyword,
                                        RegexOptions.IgnoreCase));
                if(count > -1)
                {
                    fileList[count].Add(file.FullName);
                    numMachingFiles++;
                }
            });

            var result = new string[numMachingFiles];
            var nextFile = 0;
            for(var i = numKeywords - 1; i >= 0; i--)
            {
                foreach(var file in fileList[i])
                {
                    result[nextFile++] = file;
                }
            }
            return result;
        }

        public static string[] GetFileContent(string fullFilePath)
        {
            return File.ReadAllLines(fullFilePath);
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


        private FileInfo[] GetFiles(SearchedCodeViewModel searchedCode)
        {
            return
                new DirectoryInfo(searchedCode.SearchedPath).GetFiles(
                    searchedCode.FileExtensionPattern,
                    SearchOption.AllDirectories);
        }
    }
}