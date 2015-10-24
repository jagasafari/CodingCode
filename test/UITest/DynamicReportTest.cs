namespace UITest
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using CodingCode.Common;
    using Xunit;

    public class DynamicReportTest :IDisposable
    {
        private readonly int _numRandomTests;

        public DynamicReportTest()
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
            Thread.Sleep(5000);
            var response = await TestWebApp.Client.GetAsync(string.Empty);
            Assert.True(response.IsSuccessStatusCode);
            var postAsync = await TestWebApp.CodeDatabaseModel(response);
            Assert.True(postAsync.IsSuccessStatusCode);
            for(var i = 0; i < _numRandomTests; i++)
            {
                var responseMessage =
                    await
                        TestWebApp.GetAsync(
                            @"DynamicRaport/RandomTable?assemblyName=DELL_SQLEXPRESSNorthwind");
                var readAsStringAsync =
                    await responseMessage.Content.ReadAsStringAsync();
                Assert.True(
                    TestDatabase.GetTableNames()
                        .Any(name => readAsStringAsync.Contains(name)));
            }
        }

        public void Dispose()
        {
            TestWebApp.Dispose();
        }
    }
}