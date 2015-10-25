using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FetchGitHubSamples
{
    using System.Net.Http;

    public class Program
    {

        public void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private async Task MainAsync()
        {
            //var projectZipUrl = $@"https://github.com/aspnet/{}/archive/dev.zip";
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://github.com")
            };
            int count = 1;
            while (true)
            {
                var listOfProjects = GetListOfProjectUrl(count);
                var htmlResult = await httpClient.GetAsync(GetListOfProjectUrl(count++));
                var httpContent = htmlResult.Content.ReadAsStringAsync();
            }

        }

        private static string GetListOfProjectUrl(int count)
        {
            return $"aspnet?page={count}";
        }
    }
}
