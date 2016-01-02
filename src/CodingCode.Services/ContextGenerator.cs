namespace CodingCode.Services
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.PlatformAbstractions;
    using ViewModel;

    public class ContextGenerator : IContextGenerator
    {
        private readonly ProviderServices _providerServices;
        private readonly string _appBasePath;

        public ContextGenerator(ProviderServices providerServices, IApplicationEnvironment env)
        {
            _providerServices = providerServices;
            _appBasePath = env.ApplicationBasePath;
        }

        public async Task<object> GenerateAsync(DataAccessViewModel dalInfo,
            string assemblyName)
        {
            using (var dalGenerator = _providerServices.DalGeneratorFactory.Create(dalInfo, assemblyName, _appBasePath))
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