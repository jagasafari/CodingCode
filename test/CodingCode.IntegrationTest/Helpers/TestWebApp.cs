namespace CodingCode.IntegrationTest.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common.ProcessExecution;
    using CodingCode.ViewModel;
    using Common.ProcessExecution.Abstraction;

    public class TestWebApp : IDisposable
    {
        private ITokenRetriever _tokenRetriever;
        private ILongRunningExecutor _processExecutor;
        private HttpClient _client;
        
        public TestWebApp(ILongRunningExecutorFactory longRunningExecutorFactory, ITokenRetriever tokenRetriever)
        {
            _tokenRetriever = tokenRetriever;
            _processExecutor = longRunningExecutorFactory.Create(DnxInformation.DnxPath, "web");

            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }
        
        public void Dispose()
        {
            _client.Dispose();
            _processExecutor.Dispose();
        }

        public async Task<HttpResponseMessage> GetAsync(string actionName) =>
            await _client.GetAsync(actionName);

        public async Task DeployWebApplication()
        {
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await _client.GetAsync(string.Empty);
            }
            catch (Exception)
            {
                if (responseMessage == null || responseMessage.IsSuccessStatusCode)
                    StartHostingWebApplication();
            }
        }

        public async Task<HttpResponseMessage> CodeDatabase(string htmlContent, string formActionUrl)
        {
            var formParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(nameof(DataAccessViewModel.ServerName), @"DELL\SQLEXPRESS"),
                new KeyValuePair<string, string>(nameof(DataAccessViewModel.DatabaseName), "Northwind"),
                new KeyValuePair<string, string>("__RequestVerificationToken", _tokenRetriever.RetrieveAntiForgeryToken(htmlContent))
            };

            var formUrlEncodedContent = new FormUrlEncodedContent(formParameters.ToArray());

            return await _client.PostAsync(formActionUrl, formUrlEncodedContent);
        }

        private void StartHostingWebApplication()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directoryInfo = Directory.GetParent(Directory.GetParent(currentDirectory).FullName);
            var presentationPath = Path.Combine(directoryInfo.FullName, "src", "CodingCode.Web");
            
            Directory.SetCurrentDirectory(presentationPath);
            _processExecutor.Execute();
            Directory.SetCurrentDirectory(currentDirectory);
        }
    }
}