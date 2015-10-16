namespace UITest
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using Xunit;

    public class Class1 : IDisposable
    {
        public TestDatabase TestDatabase { get; set; }
        public TestWebApp TestWebApp { get; set; }
        private readonly int _numRandomTests;
        public Class1()
        {
            _numRandomTests = 10;
            TestWebApp = new TestWebApp
            {
                Client = new HttpClient
                {
                    BaseAddress = new Uri( "http://localhost:5000" )
                },
                Controller = "Northwind"
            };
            TestDatabase = new TestDatabase();
        }

        public void Dispose()
        {
            TestWebApp.Dispose();
        }

        [Fact]
        public async void Test()
        {
            await TestWebApp.InitialiseTestAsync();

            for ( int i = 0; i < _numRandomTests; i++ )
            {
                var responseMessage = await TestWebApp.GetAsync();
                var readAsStringAsync =
                    await responseMessage.Content.ReadAsStringAsync();
                Assert.True(
                    TestDatabase.GetTableNames()
                        .Any( name => readAsStringAsync.Contains( name ) ) );
            }
        }

    }
}
