namespace CodingCode.IntegrationTest.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;

    public class TestWebApp : IDisposable
    {
        public HttpClient Client { get; set; }
        public ProcessExecutor ProcessExecutor { get; set; }
        public TokenRetriever TokenRetriever { get; set; }

        public void Dispose()
        {
            Client.Dispose();
            ProcessExecutor.Dispose();
        }

        public async Task<HttpResponseMessage> GetAsync(string actionName)
        {
            return await Client.GetAsync(actionName);
        }

        public async Task DeployWebApplication()
        {
            HttpResponseMessage responseMessage=null;
            try
            {
                responseMessage = await Client.GetAsync(string.Empty);
            }
            catch(Exception)
            {
                if(responseMessage==null || !responseMessage.IsSuccessStatusCode)
                    StartHostingWebApplication();
            }
        }

        private void StartHostingWebApplication()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directoryInfo =
                Directory.GetParent(
                    Directory.GetParent(currentDirectory).FullName);
            var presentationPath = Path.Combine(
                directoryInfo.FullName,
                "src", "CodingCode.Web");
            Directory.SetCurrentDirectory(presentationPath);
            ProcessExecutor.Execute(
                DnxInformation.GetDnx(),
                "web", 10000);
            Directory.SetCurrentDirectory(currentDirectory);
        }

        public async Task<HttpResponseMessage> CodeDatabaseModel(
            HttpResponseMessage response)
        {
            TokenRetriever.HtmlContent =
                await response.Content.ReadAsStringAsync();

            var formParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Server",
                    @"DELL\SQLEXPRESS"),
                new KeyValuePair<string, string>("Database","Northwind"),
                new KeyValuePair<string, string>(
                    "__RequestVerificationToken",
                    TokenRetriever.RetrieveAntiForgeryToken())
            };

            var formUrlEncodedContent =
                new FormUrlEncodedContent(formParameters.ToArray());
            return
                await
                    Client.PostAsync(
                        "DynamicRaport/CodeDatabaseModel",
                        formUrlEncodedContent);
        }
    }
}