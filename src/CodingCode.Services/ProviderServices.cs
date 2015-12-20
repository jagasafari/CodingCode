namespace CodingCode.Services
{
    using System;
    using CodingCode.ViewModel;
    using Common.ProcessExecution;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Contracts;
    using Common.Core;

    public class ProviderServices{
        private readonly IServiceProvider _serviceProvider;

        public ProviderServices(IApplicationEnvironment applicationEnvironment)
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                
                .AddSingleton<ProcessProviderServices>()
                .AddSingleton(typeof(DbContextWrapper))
                .AddSingleton<ModelsProvider>()
                
                .AddInstance(applicationEnvironment)
                
                .AddScoped<IQueryRequestMapper, QueryRequestMapper>()
                .AddScoped<IRandomTablePicker, RandomTablePicker>()
                
                .BuildServiceProvider();
        }
        
        public IQueryRequestMapper QueryRequestMapper=>
            Check.NotNull<IQueryRequestMapper>((IQueryRequestMapper)_serviceProvider.GetService(typeof(IQueryRequestMapper)));
        
        public IRandomTablePicker RandomTablePicker=>
            Check.NotNull<IRandomTablePicker>((IRandomTablePicker)_serviceProvider.GetService(typeof(IRandomTablePicker)));
        
        public IContextGenerator ContextGenerator=>
            Check.NotNull<IContextGenerator>(new ContextGenerator(this));     
               
        public ILoggerFactory LoggerFactory
            =>
                Check.NotNull<ILoggerFactory>((ILoggerFactory)
                    _serviceProvider.GetService(typeof(ILoggerFactory)));

        public ILogger ConsoleLogger(string name) => Check.NotNull<ILogger>(LoggerFactory
            .AddConsole(LogLevel.Information)
            .CreateLogger($"{name}"));
            
         public ModelsProvider ModelProvider =>
            Check.NotNull<ModelsProvider>((ModelsProvider)
                _serviceProvider.GetService(typeof(ModelsProvider)));

        public ProcessProviderServices ProcessProviderServices =>
            Check.NotNull<ProcessProviderServices>((ProcessProviderServices)_serviceProvider.GetService(typeof(ProcessProviderServices)));

        public IApplicationEnvironment ApplicationEnvironment =>
            Check.NotNull<IApplicationEnvironment>((IApplicationEnvironment)_serviceProvider.GetService(typeof(IApplicationEnvironment)));

        public DbContextWrapper DbContextWrapper =>
            Check.NotNull<DbContextWrapper>((DbContextWrapper)_serviceProvider.GetService(typeof(DbContextWrapper)));

        public IDalGenerator DalGenerator(DataAccessViewModel dalInfo, string assemblyName) => 
         Check.NotNull<IDalGenerator>(new DalGenerator(this){DataAccessSettings = ModelProvider.DataAccessSettings(
             dalInfo, assemblyName, ApplicationEnvironment.ApplicationBasePath)});
    }
}