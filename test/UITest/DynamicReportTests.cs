namespace UITest
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using CodingCode.Common;
    using Xunit;

    public class DynamicReportTests
    {
        private readonly int _numRandomTests;

        public DynamicReportTests()
        {
            _numRandomTests = 10;
            TestWebApp = new TestWebApp
            {
                Client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:5000")
                },
                ProcessExecutor = new ProcessExecutor
                {
                    ProcessInstance = new Process(),
                    ExpectedExit = false
                },
                TokenRetriever = new TokenRetriever
                {
                    ActionUrl = "/DynamicRaport/CodeDatabaseModel"
                }
            };
            TestDatabase = new TestDatabase();
        }

        public TestDatabase TestDatabase { get; set; }
        public TestWebApp TestWebApp { get; set; }


        [Fact]
        public async void
            CodeDatabaseModel_FollowedByRetrieving10RandomTables_IntegrationTest
            ()
        {
            await TestWebApp.DeployWebApplication();
            var response = await TestWebApp.Client.GetAsync(string.Empty);
            Assert.True(response.IsSuccessStatusCode);
            var postAsync = await TestWebApp.CodeDatabaseModel(response);
            Assert.True(postAsync.IsSuccessStatusCode);
            for(var i = 0; i < _numRandomTests; i++)
            {
                var responseMessage =
                    await
                        TestWebApp.GetAsync(
                            @"DynamicRaport/RandomTable?assemblyName=DELL_SQLEXPRESS_Northwind");
                var readAsStringAsync =
                    await responseMessage.Content.ReadAsStringAsync();
                Assert.True(
                    TestDatabase.GetTableNames()
                        .Any(name => readAsStringAsync.Contains(name)));
            }
        }
    }
}