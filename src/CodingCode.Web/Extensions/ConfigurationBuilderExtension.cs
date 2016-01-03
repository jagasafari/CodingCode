using Microsoft.Extensions.Configuration;

namespace Codingcode.Web{
    public static class ConfigurationBuilderExtensions{
        public static IConfigurationRoot BuildConfiguration(this ConfigurationBuilder builder,
            string applicationBasePath){
            return builder
                    .SetBasePath(applicationBasePath)
                    .AddEnvironmentVariables()
                    .Build();
        }
    }
}