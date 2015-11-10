namespace CodingCode.Web
{
    using Contracts;
    using Logic;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Dnx.Runtime;
    using Microsoft.Framework.Configuration;
    using Microsoft.Framework.DependencyInjection;
    using Microsoft.Framework.Logging;
    using ProcessExecution;

    public class Startup
    {
        public Startup(IHostingEnvironment env,
            IApplicationEnvironment appEnv)
        {
            var builder =
                new ConfigurationBuilder().SetBasePath(
                    appEnv.ApplicationBasePath)
                    .AddJsonFile("config.json")
                    .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddSingleton(typeof(DbContextWrapper))
                .AddScoped<IQueryRequestMapper, QueryRequestMapper>()
                .AddScoped<IRandomTablePicker, RandomTablePicker>()
                .AddScoped<IContextGenerator, ContextGenerator>()
                .AddScoped<ProcessProviderServices>()
                .AddInstance(Configuration);
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();


            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();
            app.UseRuntimeInfoPage();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}