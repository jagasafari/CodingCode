namespace CodingCode.Services
{
    using System.IO;
    using Model;
    using ViewModel;

    public class DataAccessSettingsMapper
    {
        public DataAccessViewModel DataAccessViewModel { get; set; }
        public string ApplicationBasePath { get; set; }
        public string AssemblyName { get; set; }

        public DataAccessConfigurations Map()
        {
            var dalDirectoryParent =
                Directory.GetParent(ApplicationBasePath)
                    .FullName;

            var dalDirectory = Path.Combine(
                dalDirectoryParent, AssemblyName);

            var templateDirectory = Path.Combine(
                Directory.GetParent(dalDirectory).FullName,
                "CodingCode.Web",
                "Templates");

            return new DataAccessConfigurations
            {
                DatabaseName = DataAccessViewModel.DatabaseName,
                ServerName = DataAccessViewModel.ServerName,
                AssemblyName = AssemblyName,
                ProjectDirectory = dalDirectory,
                TemplateDirectory = templateDirectory
            };
        }
    }
}