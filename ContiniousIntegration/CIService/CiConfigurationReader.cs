namespace CIService
{
    using System;
    using Microsoft.Framework.Configuration;

    public class CiConfigurationReader
    {
        private static IConfiguration configuration;

        public CiConfigurationReader()
        {
            configuration =
                new ConfigurationBuilder().AddJsonFile("config.json")
                    .Build();
        }

        public CiTestConfiguration GetTestConfiguration()
        {
            var ciTestConfiguration = new CiTestConfiguration
            {
                SolutionPath = configuration["Paths:Solution"],
                MinutesToWait = new TimeSpan(0,
                    int.Parse(configuration["Timeing:MinutesToWait"]), 0)
            };
            var count = 0;
            var next = GetTestProject(count++);
            while(next != null)
            {
                ciTestConfiguration.TestProjects.Add(next);
                next = GetTestProject(count++);
            }
            return ciTestConfiguration;
        }

        public CiMailConfiguration GetMailConfiguration()
        {
            var ciMailConfiguration = new CiMailConfiguration
            {
                Sender =
                    configuration[
                        GetKey(nameof(CiMailConfiguration.Sender))],
                Password =
                    configuration[
                        GetKey(nameof(CiMailConfiguration.Password))],
                SmtpPort =
                    int.Parse(
                        configuration[
                            GetKey(nameof(CiMailConfiguration.SmtpPort))]),
                SmtpHost =
                    configuration[
                        GetKey(nameof(CiMailConfiguration.SmtpHost))],
                Receiver =
                    configuration[
                        GetKey(nameof(CiMailConfiguration.Receiver))]
            };
            return ciMailConfiguration;
        }

        private static string GetKey(string property)
        {
            return $"EMail:{property}";
        }

        private static string GetTestProject(int count)
        {
            return configuration[$"Paths:TestProjects:{count}"];
        }
    }
}