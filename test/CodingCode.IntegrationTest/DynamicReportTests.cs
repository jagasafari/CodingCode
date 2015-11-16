namespace CodingCode.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using Helpers;
    using Xunit;
    using System.Linq;
    using ProcessExecution;

    public class DynamicReportTests :IDisposable
    {
        private readonly int _numRandomTests;

        public DynamicReportTests()
        {
            _numRandomTests = 10;
            TestWebApp = new TestWebApp(new ProcessProviderServices())
            {
                Client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:5000")
                }
            };
        }

        public IEnumerable<string> TableNames => new[]
            {
                "Categories", "CustomerCustomerDemo",
                "CustomerDemographics",
                "Customers", "EmployeeTerritories", "Employees",
                "Order_Details", "Orders", "Products", "Region",
                "Shippers", "Suppliers", "Territories", "sysdiagrams"
            };
        public TestWebApp TestWebApp { get; set; }


        [Fact]
        public async void
            CodeDatabaseModel_FollowedByRetrieving10RandomTables_IntegrationTest
            ()
        {
            // make sure web app is hosted
            await TestWebApp.DeployWebApplication();
            Thread.Sleep(5000);
            var response = await TestWebApp.Client.GetAsync(string.Empty);
            Assert.True(response.IsSuccessStatusCode);

            // test CodeDatabase Action
            var formActionUrl =
                @"http://localhost:5000/DynamicRaport/CodeDatabase";
            var antiForgeryToken = TokenRetriever.RetrieveAntiForgeryToken(
                await response.Content.ReadAsStringAsync());
            var postAsync = await TestWebApp.CodeDatabase(antiForgeryToken, formActionUrl);
            Assert.True(postAsync.IsSuccessStatusCode);

            // test RandomTable action
            for (var i = 0; i < _numRandomTests; i++)
            {
                var responseMessage =
                    await
                        TestWebApp.GetAsync(
                            @"DynamicRaport/RandomTable?assemblyName=DELL_SQLEXPRESSNorthwind");
                var readAsStringAsync =
                    await responseMessage.Content.ReadAsStringAsync();
                Assert.True(TableNames.Any(name => readAsStringAsync.Contains(name)));
            }
        }

        public void Dispose()
        {
            TestWebApp.Dispose();
        }
    }
}