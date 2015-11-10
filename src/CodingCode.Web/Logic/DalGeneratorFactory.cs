namespace CodingCode.Web.Logic
{
    using System.IO;
    using Contracts;
    using ProcessExecution;
    using ViewModels;

    public class DalGeneratorFactory : IDalGeneratorFactory
    {
        public DalInfoViewModel DalInfoViewModel { get; set; }
        public string ApplicationBasePath { get; set; }
        public string AssemblyName { get; set; }

        public DalGenerator Create()
        {
            var dalDirectoryParent =
                Directory.GetParent(ApplicationBasePath)
                    .FullName;

            var dalDirectory = Path.Combine(
                dalDirectoryParent, AssemblyName);

            var templateDirectory = Path.Combine(
                Directory.GetParent(dalDirectory).FullName, "CodingCode.Web",
                "Templates");

            return new DalGenerator
            {
                Database = DalInfoViewModel.Database,
                Server = DalInfoViewModel.Server,
                AssemblyName = AssemblyName,
                DalDirectory = dalDirectory,
                TemplateDirectory = templateDirectory,
                ProcessProviderServices = new ProcessProviderServices()
            };
        }
    }
}