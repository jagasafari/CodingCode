namespace CodingCode.Web.Logic
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Dnx.Runtime;
    using ViewModels;

    public class ContextGenerator : IContextGenerator
    {
        private readonly IApplicationEnvironment _applicationEnvironment;

        public ContextGenerator(
            IApplicationEnvironment applicationEnvironment)
        {
            _applicationEnvironment = applicationEnvironment;
        }

        public async Task<object> GenerateAsync(DalInfoViewModel dalInfo,
            string assemblyName)
        {
            var dalGeneratorFactory = new DalGeneratorFactory
            {
                ApplicationBasePath =
                    _applicationEnvironment.ApplicationBasePath,
                DalInfoViewModel = dalInfo,
                AssemblyName = assemblyName
            };

            using(
                var dalGenerator = dalGeneratorFactory.Create())
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