namespace CodingCode.Web
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Services;
    using Codingcode.Web;
    using Contracts;

    public class Startup
    {
        public Startup(IApplicationEnvironment appEnv)
        {
            Configuration = new ConfigurationBuilder()
                .BuildConfiguration(appEnv.ApplicationBasePath);
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddLogging()
                .AddTransient<IQueryRequestMapper, QueryRequestMapper>()
                .AddTransient<IRandomTablePicker, RandomTablePicker>()
                .AddTransient<IContextGenerator, ContextGenerator>()
                .AddSingleton(typeof(DbContextWrapper))
                .AddSingleton<ProviderServices>();
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
                .UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=DataAccessScaffold}/{action=CodeDatabase}/{id?}");
            });
        }
    }
}