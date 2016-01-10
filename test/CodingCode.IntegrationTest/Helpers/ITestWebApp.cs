namespace CodingCode.IntegrationTest{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ITestWebApp : IDisposable {
        Task DeployWebApplication();
        Task<HttpResponseMessage> GetAsync(string actionName);
        Task<HttpResponseMessage> PostAsync(string formActionUrl, 
            IEnumerable<KeyValuePair<string, string>> formUrlEncodedContent);
    }
}