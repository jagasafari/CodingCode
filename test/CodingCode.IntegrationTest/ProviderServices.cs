using System;
using CodingCode.IntegrationTest.Helpers;
using Common.ProcessExecution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodingCode.IntegrationTest
{
    public class ProviderServices{
        private IServiceProvider _serviceProvider;
        public ProviderServices()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddProceesProviderServices()
                .AddTransient<TestWebApp>()
                .AddTransient<ITokenRetriever, TokenRetriever>()
                .BuildServiceProvider();
                
           _serviceProvider.GetService<ILoggerFactory>().AddConsole();
        }
        
        public TestWebApp TestWebApp => _serviceProvider.GetService<TestWebApp>();
        
    }
}