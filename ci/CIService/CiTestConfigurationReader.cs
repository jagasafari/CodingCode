namespace CIService
{
    using Microsoft.Framework.Configuration;

    public class CiTestConfigurationReader
    {
        private static IConfiguration configuration;
        public static CiTestConfiguration GetConfiguration()
        {
            configuration =
                new ConfigurationBuilder().AddJsonFile("config.json")
                    .Build();

            var ciTestConfiguration = new CiTestConfiguration
            {
                SolutionPath = configuration["Paths:Solution"],
                MinutesToWait =
                    int.Parse(configuration["Timeing:MinutesToWait"])
            };
            var count = 0;
            var next = GetTestProject(count++);
            while (next != null)
            {
                ciTestConfiguration.TestProjects.Add(next);
                next = GetTestProject(count++);
            }
            return ciTestConfiguration;
        }

        private static string GetTestProject(int count)
        {
            return configuration[$"Paths:TestProjects:{count}"];
        }
    }
}