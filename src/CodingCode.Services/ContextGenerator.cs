namespace CodingCode.Services
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.PlatformAbstractions;
    using Contracts;
    using ViewModel;
    using Microsoft.Extensions.Logging;
    using ProcessExecution;

    public class ContextGenerator : IContextGenerator
    {
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly DalGenerator _dalGenerator;
        private readonly ILogger _logger;
        private readonly ProviderModel _providerModel;

        public ContextGenerator(
            IApplicationEnvironment applicationEnvironment,
            ILoggerFactory loggerFactory)
        {
            _applicationEnvironment = applicationEnvironment;
            _logger=loggerFactory.CreateLogger(nameof(ContextGenerator));
            _providerModel = new ProviderModel();
        }

        public async Task<object> GenerateAsync(DalInfoViewModel dalInfo,
            string assemblyName)
        {
            var dataAccessSettings = _providerModel.DataAccessSettings(dalInfo, assemblyName,
                _applicationEnvironment.ApplicationBasePath);

            using (
                var dalGenerator = new DalGenerator(_logger){
                    DataAccessSettings=dataAccessSettings,
                    ProcessProviderServices = new ProcessProviderServices()})
            {
                dalGenerator.CreateDalDirectory();

                dalGenerator.CopyProjectJson();

                await dalGenerator.RestoreAsync();

                await dalGenerator.ScaffoldAsync();

                dalGenerator.CodeContext();

                await dalGenerator.CodeEntitiesAsync();

                await dalGenerator.BuildAsync();

                return dalGenerator.InstantiateDbContext();
            }
        }
    }
}