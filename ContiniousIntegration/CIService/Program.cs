namespace CIService
{
    using System;
    using Microsoft.Framework.Logging;

    public class Program
    {
        private ILogger _logger;
        private Mailer _mailer;
        private TestRunner _testRunner;

        public Program()
        {
            InitializeCiProgram();
        }

        public void Main(string[] args)
        {
            try
            {
                _testRunner.Run();
            }
            catch(Exception e)
            {
                _logger.Error($"Excepion cought : {Environment.NewLine}" +
                              $"{e}");
                var mailMessage = _mailer.CreateMessageEmail(e.StackTrace,"Error");
                _mailer.SendReportEmail(mailMessage);
            }
        }

        private void InitializeCiProgram()
        {
            _logger = new LoggerFactory
            {
                MinimumLevel = LogLevel.Debug
            }.AddConsole().CreateLogger("CI");

            var ciConfigurationReader = new CiConfigurationReader();
            _mailer =
                new Mailer(ciConfigurationReader.GetMailConfiguration());
            _testRunner = new TestRunner(_logger, ciConfigurationReader, _mailer);
            _logger.Info("CI Service Initialized!");
        }
    }
}