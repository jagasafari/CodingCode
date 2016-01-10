namespace CodingCode.Services
{
    using System.IO;
    using CodingCode.Abstraction;
    using Microsoft.Extensions.OptionsModel;
    using Model;
    using ViewModel;

    public class DataAccessSettingsMapper : IDataAccessSettingsMapper
    {
        private DalProjectOptions _options;
        public DataAccessSettingsMapper(IOptions<DalProjectOptions> options)
        {
            _options = options.Value;
        }
        public DataAccessConfigurations Map(DataAccessViewModel dataAccessViewModel, string assemblyName)
        {
            var dalDirectory = Path.Combine(_options.DalParentDirectory, assemblyName);

            var templateDirectory = Path.Combine(Directory.GetParent(dalDirectory).FullName, _options.TemplateDirectory);

            return new DataAccessConfigurations
            {
                DatabaseName = dataAccessViewModel.DatabaseName,
                ServerName = dataAccessViewModel.ServerName,
                AssemblyName = assemblyName,
                ProjectDirectory = dalDirectory,
                TemplateDirectory = templateDirectory
            };
        }
    }
}