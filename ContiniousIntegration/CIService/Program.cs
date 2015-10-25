namespace CIService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;
    using CodingCode.Common;
    using Microsoft.Framework.Logging;

    public class Program
    {
        private readonly ILogger _logger;
        private readonly CiTestConfiguration _testConfiguration;


        public Program()
        {
            _testConfiguration =
                CiTestConfigurationReader.GetConfiguration();
            _logger = new LoggerFactory
            {
                MinimumLevel = LogLevel.Debug
            }.AddConsole().CreateLogger("CI");
            _logger.Info("CI Service Initialized!");
        }


        public void Main(string[] args)
        {
            var timeSpan = new TimeSpan(0, _testConfiguration.MinutesToWait,
                0);
            while(true)
            {
                if(ModifiedFileFound())
                {
                    RunTests();
                }
                else
                {
                    _logger.Info("Nothing have changed");
                }

                _logger.Info(
                    $"Going to sleep for {_testConfiguration.MinutesToWait} minutes");
                Thread.Sleep(timeSpan);
            }
        }

        private void RunTests()
        {
            var stringBuilder = new StringBuilder();
            foreach(var projectPath in GetTestProjects())
            {
                var processExecutor = new ProcessExecutor
                {
                    ExpectedExit = true
                };
                _logger.Info($"Testing {projectPath} project");
                processExecutor.ExecuteAndWait(DnxTool.GetDnx(),
                    $"-p {projectPath} test",
                    x => x.Equals("Failed"));
                _logger.Info("Tests Completed!");
                stringBuilder.Append(processExecutor.Output);
            }
            SendReportEmail(stringBuilder);
        }

        private void SendReportEmail(StringBuilder stringBuilder)
        {
            _logger.Info("Sending Report email");
            using(var smtpClient = CreateSmtClient())
            {
                smtpClient.Send(CreateMessageEmail(stringBuilder));
            }
        }

        private MailMessage CreateMessageEmail(StringBuilder stringBuilder)
        {
            return new MailMessage(_testConfiguration.Sender,
                _testConfiguration.Receiver, $"CI Report {DateTime.UtcNow}",
                stringBuilder.ToString());
        }

        private SmtpClient CreateSmtClient()
        {
            return new SmtpClient(_testConfiguration.SmtpHost,
                _testConfiguration.SmtpPort)
            {
                EnableSsl = true,
                Credentials =
                    new NetworkCredential(_testConfiguration.Sender,
                        _testConfiguration.Password)
            };
        }

        private bool ModifiedFileFound()
        {
            var minutesFromLastRun =
                DateTime.UtcNow.AddMinutes(
                    -_testConfiguration.MinutesToWait);

            return
                AnyFileModified(new[] {"*.cs", "*.cshtml"},
                    _testConfiguration.SolutionPath,
                    minutesFromLastRun);
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

        public bool AnyFileModified(string[] patterns,
            string solutionPath, DateTime minutesFromLastRun)
        {
            foreach(var pattern in patterns)
            {
                var files =
                    new DirectoryInfo(solutionPath).GetFiles(pattern,
                        SearchOption.AllDirectories);
                _logger.Info(
                    $"{files.Length} {pattern} under the solution");
                var modifiedFileFound =
                    files.Any(f => f.LastWriteTimeUtc < minutesFromLastRun);
                _logger.Info(
                    $"{modifiedFileFound} that at least one of them was modified within {minutesFromLastRun} minutes");
                if(modifiedFileFound)
                    return true;
            }
            return false;
        }
    }
}