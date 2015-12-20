namespace CodingCode.Services
{
    using Model;
    using ViewModel;
    using Common.Core;
    
    public class ModelsProvider
    {
        public DataAccessConfigurations DataAccessSettings(
            DataAccessViewModel dataAccessViewModel,
            string assemblyName,
            string applicationBasePath) =>
            Check.NotNull<DataAccessSettingsMapper>(new DataAccessSettingsMapper
            {
                DataAccessViewModel = dataAccessViewModel,
                ApplicationBasePath = applicationBasePath,
                AssemblyName = assemblyName
            }).Map();
    }
}