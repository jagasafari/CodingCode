namespace CIService
{
    using Microsoft.Framework.Logging;

    public class Program
    {
        private ILogger _logger;

        private TestRunner _testRunner;

        public Program()
        {
            InitializeCiProgram();
        }

        public void Main(string[] args)
        {
            _testRunner.Run();
        }

        private void InitializeCiProgram()
        {
            _logger = new LoggerFactory
            {
                MinimumLevel = LogLevel.Debug
            }.AddConsole().CreateLogger("CI");

            _testRunner = new TestRunner(_logger, new CiConfigurationReader());
            _logger.Info("CI Service Initialized!");
        }
    }
}