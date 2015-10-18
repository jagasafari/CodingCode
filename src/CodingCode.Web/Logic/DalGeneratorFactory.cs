namespace CodingCode.Web.Logic
{
    using System.IO;
    using Contracts;
    using ViewModel;

    public class DalGeneratorFactory : IDalGeneratorFactory
    {
        public DalInfoViewModel DalInfoViewModel { get; set; }

        public DalGenerator Create()
        {
            var dalDirectoryParent =
                Directory.GetParent(DalInfoViewModel.AssemblyBasePath)
                    .FullName;

            var dalDirectory = Path.Combine(
                dalDirectoryParent, DalInfoViewModel.AssemblyName);

            var templateDirectory = Path.Combine(
                Directory.GetParent(dalDirectory).FullName, "CodingCode.Web",
                "Templates");

            return new DalGenerator
            {
                DatabaseName = DalInfoViewModel.DatabaseName,
                AssemblyName = DalInfoViewModel.AssemblyName,
                ConnectionString = DalInfoViewModel.ConnectionString,
                DalDirectory = dalDirectory,
                TemplateDirectory = templateDirectory
            };
        }
    }
}