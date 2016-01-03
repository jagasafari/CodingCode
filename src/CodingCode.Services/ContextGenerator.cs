namespace CodingCode.Services
{
    using System.Threading.Tasks;
    using CodingCode.Abstraction;
    using Microsoft.Extensions.PlatformAbstractions;
    using ViewModel;

    public class ContextGenerator : IContextGenerator
    {
        private readonly IDalGeneratorFactory _dalGeneratorFactory;
        private readonly string _appBasePath;

        public ContextGenerator(IDalGeneratorFactory dalGeneratorFactory, IApplicationEnvironment env)
        {
            _dalGeneratorFactory = dalGeneratorFactory;
            _appBasePath = env.ApplicationBasePath;
        }

        public async Task<object> GenerateAsync(DataAccessViewModel dalInfo,
            string assemblyName)
        {
            using (var dalGenerator = _dalGeneratorFactory.Create(dalInfo, assemblyName, _appBasePath))
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