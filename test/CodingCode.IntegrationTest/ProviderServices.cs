using System;
using CodingCode.IntegrationTest.Helpers;
using Common.ProcessExecution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodingCode.IntegrationTest
{
    public class ProviderServices{
        private IServiceProvider _serviceProvider;
        public ProviderServices()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json").Build();
            
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddProceesProviderServices()
                .AddTransient<ITestWebApp, TestWebApp>()
                .AddTransient<ITokenRetriever, TokenRetriever>()
                
                .AddOptions()
                .Configure<TestOptions>(configuration)
                .BuildServiceProvider();
                
           _serviceProvider.GetService<ILoggerFactory>().AddConsole().MinimumLevel=LogLevel.Debug;
        }
        
        public ITestWebApp TestWebApp => _serviceProvider.GetService<ITestWebApp>();
        public ITokenRetriever TokenRetriever => _serviceProvider.GetService<ITokenRetriever>();
        
        public ILogger<DynamicReportTests> TestLogger => _serviceProvider.GetService<ILogger<DynamicReportTests>>();
        
    }
}