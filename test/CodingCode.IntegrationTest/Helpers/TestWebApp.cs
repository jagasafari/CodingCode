namespace CodingCode.IntegrationTest.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Common.ProcessExecution;
    using Common.ProcessExecution.Model;
    using CodingCode.ViewModel;

    public class TestWebApp : IDisposable
    {
        public TestWebApp(ProcessProviderServices processProviderServices)
        {
            var instructions = new ProcessInstructions
            {
                Program = DnxInformation.DnxPath,
                Arguments = "web"
            };
            var logger = new LoggerFactory()
                .AddConsole(LogLevel.Information)
                .CreateLogger(nameof(TestWebApp));
            ProcessExecutor =
                processProviderServices.LivingProcessExecutor(instructions, logger);
        }

        public HttpClient Client { get; set; }
        private LivingProcessExecutor ProcessExecutor { get; }

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
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await Client.GetAsync(string.Empty);
            }
            catch(Exception)
            {
                if(responseMessage == null ||
                   ! responseMessage.IsSuccessStatusCode)
                    StartHostingWebApplication();
            }
        }

        public async Task<HttpResponseMessage> CodeDatabase(string antiForgeryToken, string formActionUrl)
        {
            var formParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(nameof(DataAccessViewModel.ServerName),
                    @"DELL\SQLEXPRESS"),
                new KeyValuePair<string, string>(nameof(DataAccessViewModel.DatabaseName),
                     "Northwind"),
                new KeyValuePair<string, string>(
                    "__RequestVerificationToken", antiForgeryToken)
            };

            var formUrlEncodedContent =
                new FormUrlEncodedContent(formParameters.ToArray());

            var re= await
                    Client.PostAsync(formActionUrl,
                        formUrlEncodedContent);
            return re;
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
            ProcessExecutor.Execute();
            Directory.SetCurrentDirectory(currentDirectory);
        }
    }
}