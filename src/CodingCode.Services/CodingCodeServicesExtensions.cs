namespace CodingCode.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using CodingCode.Abstraction;
    using Common.ProcessExecution;

    public static class CodingCodeServicesExtensions{
        public static IServiceCollection AddCodingCodeServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddProceesProviderServices()
                
                .AddSingleton<ModelsProvider>()
                
                .AddTransient<IDalGeneratorFactory, DalGeneratorFactory>()
                .AddTransient<IQueryRequestMapper, QueryRequestMapper>()
                .AddTransient<IRandomTablePicker, RandomTablePicker>()
                .AddTransient<IContextGenerator, ContextGenerator>()
                
                .AddSingleton(typeof(DbContextWrapper));
        }
    }
}