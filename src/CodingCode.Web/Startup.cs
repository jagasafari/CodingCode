namespace CodingCode.Web
{
    using Contracts;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ProcessExecution;
    using Services;

    public class Startup
    {
        public Startup(IApplicationEnvironment appEnv)
        {
            Configuration = new ConfigurationBuilder()
                    .SetBasePath(appEnv.ApplicationBasePath)
                    .AddJsonFile("config.json")
                    .AddEnvironmentVariables()
                    .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddLogging()
                .AddSingleton(typeof(DbContextWrapper))
                .AddSingleton<ProcessProviderServices>()
                .AddScoped<IQueryRequestMapper, QueryRequestMapper>()
                .AddScoped<IRandomTablePicker, RandomTablePicker>()
                .AddScoped<IContextGenerator, ContextGenerator>()
                .AddInstance(Configuration);
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();

            app
                .UseBrowserLink()
                .UseDeveloperExceptionPage()
                .UseStaticFiles()
                .UseRuntimeInfoPage()
                .UseMvcWithDefaultRoute();
        }
    }
}