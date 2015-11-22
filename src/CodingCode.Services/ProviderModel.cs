namespace CodingCode.Services
{
    using Model;
    using ViewModel;

    public class ProviderModel
    {
        public DataAccessSettings DataAccessSettings(DalInfoViewModel dalInfo,
            string assemblyName, string applicationBasePath) =>
            new DataAccessSettingsMapper
            {
                DalInfoViewModel = dalInfo,
                ApplicationBasePath = applicationBasePath,
                AssemblyName = assemblyName
            }.Map();
    }
}