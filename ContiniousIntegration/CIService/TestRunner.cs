namespace CIService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CodingCode.Common;
    using Microsoft.Framework.Logging;

    public class TestRunner
    {
        private readonly ModifiedFileFounder _founder;
        private readonly ILogger _logger;
        private readonly Mailer _mailer;
        private readonly CiTestConfiguration _testConfiguration;
        private DateTime _lastRunTime;

        public TestRunner(ILogger logger, CiConfigurationReader ciConfigurationReader, Mailer mailer)
        {
            _logger = logger;
            _testConfiguration =
                ciConfigurationReader.GetTestConfiguration();
            _mailer = mailer;
            _lastRunTime = DateTime.UtcNow.AddYears(-1000);
            _founder = new ModifiedFileFounder(_logger,
                _testConfiguration.SolutionPath);
        }

        public void Run()
        {
            while(true)
            {
                if(_founder.Search(_lastRunTime))
                {
                    var testResults = RunTests();
                    _logger.Info("Sending Report email");
                    _mailer.SendReportEmail(_mailer.CreateMessageEmail(testResults, "Report"));
                }
                else
                {
                    _logger.Info("Nothing have changed");
                }

                _lastRunTime = DateTime.UtcNow;
                _logger.Info(
                    $"Going to sleep for {_testConfiguration.MinutesToWait} minutes");
                Thread.Sleep(_testConfiguration.MinutesToWait);
            }
        }

        public string RunTests()
        {
            var stringBuilder = new StringBuilder();
            Parallel.ForEach(GetTestProjects(),
                f => { RunTest(f, stringBuilder); });
            return stringBuilder.ToString();
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

        private void RunTest(string testProjectPath,
            StringBuilder stringBuilder)
        {
            var processExecutor = new ProcessExecutor
            {
                ExpectedExit = true
            };
            _logger.Info($"Testing {testProjectPath} project");
            processExecutor.ExecuteAndWait(DnxTool.GetDnx(),
                $"-p {testProjectPath} test",
                x => x.Equals("Failed"));
            _logger.Info("Tests Completed!");
            stringBuilder.Append(processExecutor.Output);
        }
    }
}