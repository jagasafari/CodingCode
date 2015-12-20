namespace CodingCode.Services
{
    using System.Threading.Tasks;
    using Contracts;
    using ViewModel;

    public class ContextGenerator : IContextGenerator
    {
        private readonly ProviderServices _providerServices;

        public ContextGenerator(ProviderServices providerServices)
        {
            _providerServices = providerServices;
        }

        public async Task<object> GenerateAsync(DataAccessViewModel dalInfo,
            string assemblyName)
        {
            using (var dalGenerator = _providerServices.DalGenerator(dalInfo, assemblyName))
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