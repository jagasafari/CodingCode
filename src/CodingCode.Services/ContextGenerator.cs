namespace CodingCode.Services
{
    using System.Threading.Tasks;
    using CodingCode.Abstraction;
    using ViewModel;

    public class ContextGenerator : IContextGenerator
    {
        private readonly IDalGeneratorFactory _dalGeneratorFactory;

        public ContextGenerator(IDalGeneratorFactory dalGeneratorFactory)
        {
            _dalGeneratorFactory = dalGeneratorFactory;
        }

        public async Task<object> GenerateAsync(DataAccessViewModel dalInfo, string assemblyName)
        {
            using (var dalGenerator = _dalGeneratorFactory.Create(dalInfo, assemblyName))
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