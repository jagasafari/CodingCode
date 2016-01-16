namespace CodingCode.IntegrationTest
{
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;
    using System.Linq;
    using CodingCode.ViewModel;
    using Microsoft.Extensions.Logging;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Common.ProcessExecution;
    using CodingCode.IntegrationTest.Helpers;

    public class DynamicReportTests
    {
        private readonly int _numRandomTests;
        private IServiceProvider _serviceProvider;

        public DynamicReportTests()
        {
            _numRandomTests = 10;

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

            _serviceProvider.GetService<ILoggerFactory>().AddConsole().MinimumLevel = LogLevel.Debug;
        }

        public IEnumerable<string> TableNames => new[]
            {
                "Categories", "CustomerCustomerDemo",
                "CustomerDemographics",
                "Customers", "EmployeeTerritories", "Employees",
                "Order_Details", "Orders", "Products", "Region",
                "Shippers", "Suppliers", "Territories", "sysdiagrams"
            };

        [Fact]
        public async void DeployWebApp_Response_Successfull()
        {
            using (var testWebApp = _serviceProvider.GetService<ITestWebApp>())
            {
                // make sure web app is hosted
                await testWebApp.DeployWebApplication();
                Thread.Sleep(5000);
                var response = await testWebApp.GetAsync(string.Empty);
                Assert.True(response.IsSuccessStatusCode);
            }
        }

        [Fact]
        public async void CodeDatabaseModel_FollowedByRetrieving10RandomTables_IntegrationTest()
        {
            using (var testWebApp = _serviceProvider.GetService<ITestWebApp>())
            {
                // make sure web app is hosted
                await testWebApp.DeployWebApplication();
                Thread.Sleep(5000);
                var response = await testWebApp.GetAsync(string.Empty);

                // test CodeDatabase Action
                var formActionUrl = @"http://localhost:5000/DataAccessScaffold/CodeDatabase";
                var htmlContent = await response.Content.ReadAsStringAsync();

                var formParameters = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(nameof(DataAccessViewModel.ServerName), @"DELL\SQLEXPRESS"),
                    new KeyValuePair<string, string>(nameof(DataAccessViewModel.DatabaseName), "Northwind"),
                    new KeyValuePair<string, string>("__RequestVerificationToken", _serviceProvider.GetService<ITokenRetriever>().RetrieveAntiForgeryToken(htmlContent))
                };

                var postAsync = await testWebApp.PostAsync(formActionUrl, formParameters);
                Assert.True(postAsync.IsSuccessStatusCode);

                // test RandomTable action
                for (var i = 0; i < _numRandomTests; i++)
                {
                    var responseMessage = await testWebApp.GetAsync(@"DynamicRaport/RandomTable?assemblyName=DELL_SQLEXPRESSNorthwind");
                    var readAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                    Assert.True(TableNames.Any(name => readAsStringAsync.Contains(name)));
                }
            }
        }
    }
}