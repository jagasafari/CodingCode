namespace CIService
{
    using System;
    using System.IO;
    using Microsoft.Framework.Logging;
    using System.Linq;
    internal class ModifiedFileFounder
    {
        private readonly ILogger _logger;
        private readonly string _solutionPath;

        public ModifiedFileFounder(ILogger logger, string solutionPath)
        {
            _logger = logger;
            _solutionPath = solutionPath;
        }

        internal bool Search(DateTime lastRunTime)
        {
            return
                AnyFileModified(new[] { "*.cs", "*.cshtml" },
                    lastRunTime);
        }
        private bool AnyFileModified(string[] patterns, DateTime lastRunTime)
        {
            foreach (var pattern in patterns)
            {
                var files =
                    new DirectoryInfo(_solutionPath).GetFiles(pattern,
                        SearchOption.AllDirectories);
                _logger.Info(
                    $"{files.Length} {pattern} under the solution");
                var modifiedFileFound =
                    files.Any(f => f.LastWriteTimeUtc > lastRunTime);
                _logger.Info(
                    $"{modifiedFileFound} that at least one of them was modified since {lastRunTime} minutes");
                if (modifiedFileFound)
                    return true;
            }
            return false;
        }
    }
}