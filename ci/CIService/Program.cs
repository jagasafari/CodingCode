namespace CIService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using CodingCode.Common;

    public class Program
    {
        private readonly CiTestConfiguration _testConfiguration;

        public Program()
        {
            _testConfiguration =
                CiTestConfigurationReader.GetConfiguration();
        }

        public void Main(string[] args)
        {
            var timeSpan = new TimeSpan(0, _testConfiguration.MinutesToWait,
                0);
            while(true)
            {
                var minutesFromLastRun =
                    DateTime.UtcNow.AddMinutes(
                        -_testConfiguration.MinutesToWait);
                if(! GetFiles(_testConfiguration.SolutionPath)
                    .Any(f => f.LastWriteTimeUtc < minutesFromLastRun))
                    continue;

                foreach(var projectPath in GetTestProjects())
                {
                    var processExecutor = new ProcessExecutor()
                    {
                        ExpectedExit = true
                    };
                    processExecutor.ExecuteAndWait(DnxTool.GetDnx(),
                        $"-p {projectPath} test",
                        x => x.Equals("Failed"));
                }
                Thread.Sleep(timeSpan);
            }
        }

        private IEnumerable<string> GetTestProjects()
        {
            return _testConfiguration.TestProjects.Select(
                testProject =>
                    $@"""{
                        Path.Combine(
                            _testConfiguration.SolutionPath,
                            testProject)}""");
        }

        public static FileInfo[] GetFiles(string solutionPath)
        {
            return new DirectoryInfo(solutionPath).GetFiles("*.*",
                SearchOption.AllDirectories);
        }
    }
}