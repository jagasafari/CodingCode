namespace CodingCode.IntegrationTest
{
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;
    using System.Linq;

    public class DynamicReportTests
    {
        private readonly int _numRandomTests;
        private ProviderServices _providerServices;

        public DynamicReportTests()
        {
            _numRandomTests = 2;
            _providerServices = new ProviderServices();
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
        public async void CodeDatabaseModel_FollowedByRetrieving10RandomTables_IntegrationTest()
        {
            using (var testWebApp = _providerServices.TestWebApp)
            {
                // make sure web app is hosted
                await testWebApp.DeployWebApplication();
                Thread.Sleep(5000);
                var response = await testWebApp.GetAsync(string.Empty);
                Assert.True(response.IsSuccessStatusCode);

                // test CodeDatabase Action
                var formActionUrl = @"http://localhost:5000/DataAccessScaffold/CodeDatabase";
                
                var postAsync = await testWebApp.CodeDatabase(await response.Content.ReadAsStringAsync(), formActionUrl);
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