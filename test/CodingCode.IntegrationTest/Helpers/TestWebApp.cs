namespace CodingCode.IntegrationTest.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common.ProcessExecution.Abstraction;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.OptionsModel;

    public class TestWebApp : ITestWebApp
    {
        private ILongRunningExecutor _processExecutor;
        private HttpClient _client;
        
        public TestWebApp(ILongRunningExecutorFactory longRunningExecutorFactory, 
            IOptions<TestOptions> options, ILogger<TestWebApp> logger)
        {
            var testOptions = options.Value;
            _processExecutor = longRunningExecutorFactory.Create(testOptions.Dnx, $"-p {testOptions.ProjectToTest} web");

            _client = new HttpClient
            {
                BaseAddress = new Uri(testOptions.WebBaseUri)
            };
        }
        
        public void Dispose()
        {
            _client.Dispose();
            _processExecutor.Dispose();
        }
        
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
                     _processExecutor.Execute();
            }
        }
        
        public async Task<HttpResponseMessage> GetAsync(string actionName) =>
            await _client.GetAsync(actionName);


        public async Task<HttpResponseMessage> PostAsync(string formActionUrl, 
            IEnumerable<KeyValuePair<string, string>> formUrlEncodedContent) =>
                await _client.PostAsync(formActionUrl, new FormUrlEncodedContent(formUrlEncodedContent));
        
    }
}