namespace Codingcode.Web
{
    using Microsoft.Extensions.Configuration;
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationRoot BuildConfiguration(this ConfigurationBuilder builder, string applicationBasePath) =>
            builder
                .SetBasePath(applicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();
    }
}