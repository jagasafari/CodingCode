﻿namespace Presentation
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
        public Startup( IHostingEnvironment env,
            IApplicationEnvironment appEnv )
        {
            var builder =
                new ConfigurationBuilder( appEnv.ApplicationBasePath )
                    .AddJsonFile( "config.json" )
                    .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices( IServiceCollection services )
        {
            services.AddMvc();
            services.AddSingleton( typeof ( DbContextWrapper ) );
            services.AddInstance( Configuration );
            services.AddScoped<IQueryRequestMapper, QueryRequestMapper>();
            services.AddScoped<IRandomTablePicker, RandomTablePicker>();
            services.AddScoped<IDalGeneratorFactory, DalGeneratorFactory>();
        }

        public void Configure( IApplicationBuilder app,
            IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();


            app.UseBrowserLink();
            app.UseErrorPage();
            app.UseRuntimeInfoPage();

            app.UseStaticFiles();

            app.UseMvc(
                routes =>
                {
                    routes.MapRoute( "default",
                        "{controller=Home}/{action=Index}/{id?}" );
                } );
        }
    }
}