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
            services.AddInstance(Configuration);
            services.AddScoped<IQueryRequestMapper, QueryRequestMapper>();
            services.AddScoped<IRandomTablePicker, RandomTablePicker>();
            services.AddScoped<IContextGenerator, ContextGenerator>();
            services.AddSingleton(typeof(DbContextWrapper));
        }

        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();


            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();
            //app.UseExceptionHandler((x) => { });//to do
            app.UseRuntimeInfoPage();

            app.UseStaticFiles();

            app.UseRequestLocalization();
            app.UseMvc(
                routes =>
                {
                    routes.MapRoute("default",
                        "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}