namespace CodingCode.Services
{
    using System.IO;
    using Model;
    using ViewModel;

    public class DataAccessSettingsMapper
    {
        public DalInfoViewModel DalInfoViewModel { get; set; }
        public string ApplicationBasePath { get; set; }
        public string AssemblyName { get; set; }

        public DataAccessSettings Map()
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

            return new DataAccessSettings
            {
                Database = DalInfoViewModel.Database,
                Server = DalInfoViewModel.Server,
                AssemblyName = AssemblyName,
                DalDirectory = dalDirectory,
                TemplateDirectory = templateDirectory
            };
        }
    }
}