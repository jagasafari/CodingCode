namespace UITest
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Share;

    public class TestWebApp : IDisposable
    {
        private readonly ProcessExecutor _processExecutor;

        public TestWebApp()
        {
            _processExecutor = new ProcessExecutor
            {
                ProcessInstance = new Process(),
                ExpectedExit = false
            };
        }

        public HttpClient Client { get; set; }

        public string Controller { get; set; }

        public void Dispose()
        {
            Client.Dispose();
            _processExecutor.Dispose();
        }

        public async Task<HttpResponseMessage> GetAsync()
        {
            return await Client.GetAsync( Controller );
        }

        public async Task InitialiseTestAsync()
        {
            try
            {
                var responseMessage = await GetAsync();
                if ( !responseMessage.IsSuccessStatusCode )
                    throw new Exception( "UnsuccessfullStatusCode" );
            }
            catch ( Exception )
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var directoryInfo =
                    Directory.GetParent(
                        Directory.GetParent( currentDirectory ).FullName );
                var presentationPath = Path.Combine(
                    directoryInfo.FullName,
                    "src", "Presentation" );
                Directory.SetCurrentDirectory( presentationPath );

                _processExecutor.Execute(
                    Path.Combine( DnxTool.GetDnxPath(), "dnx.exe" ),
                    "web", 10000 );
                Directory.SetCurrentDirectory( currentDirectory );
            }
        }
    }
}