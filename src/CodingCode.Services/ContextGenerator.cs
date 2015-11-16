namespace CodingCode.Services
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Dnx.Runtime;
    using ViewModel;

    public class ContextGenerator : IContextGenerator
    {
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly CodingCodeProviderServices _codingCodeProviderServices;
        private readonly ProviderModel _providerModel;

        public ContextGenerator(
            IApplicationEnvironment applicationEnvironment)
        {
            _applicationEnvironment = applicationEnvironment;
            _codingCodeProviderServices = new CodingCodeProviderServices();
            _providerModel = new ProviderModel();
        }

        public async Task<object> GenerateAsync(DalInfoViewModel dalInfo,
            string assemblyName)
        {
            var dataAccessSettings = _providerModel.DataAccessSettings(dalInfo, assemblyName,
                _applicationEnvironment.ApplicationBasePath);

            using (
                var dalGenerator = _codingCodeProviderServices.DalGenerator(dataAccessSettings))
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