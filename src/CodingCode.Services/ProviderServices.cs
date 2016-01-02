namespace CodingCode.Services
{
    using System;
    using Common.ProcessExecution;
    using Microsoft.Extensions.DependencyInjection;
    using Common.Core;
    using Microsoft.Extensions.Logging;

    public class ProviderServices{
        private readonly IServiceProvider _serviceProvider;

        public ProviderServices()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ProcessProviderServices>()
                .AddSingleton<ModelsProvider>()
                .AddTransient<DalGeneratorFactory>()
                
                .BuildServiceProvider();
                
            _serviceProvider.GetService<ILoggerFactory>().AddConsole().MinimumLevel=LogLevel.Information;
        }
        
        public DalGeneratorFactory DalGeneratorFactory =>
            Check.NotNull<DalGeneratorFactory>(_serviceProvider.GetService<DalGeneratorFactory>());
    }
}