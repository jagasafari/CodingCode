namespace CodingCode.Logic
{
    using System.IO;
    using Contracts;

    public class DalGeneratorFactory : IDalGeneratorFactory
    {
        public DalInfo DalInfo { get; set; }

        public DalGenerator Create()
        {
            var dalDirectoryParent =
                Directory.GetParent(DalInfo.AssemblyBasePath)
                    .FullName;

            var dalDirectory = Path.Combine(
                dalDirectoryParent, DalInfo.AssemblyName);

            var templateDirectory = Path.Combine(
                Directory.GetParent(dalDirectory).FullName, "CodingCode",
                "Templates");

            return new DalGenerator
            {
                DatabaseName = DalInfo.DatabaseName,
                AssemblyName = DalInfo.AssemblyName,
                ConnectionString = DalInfo.ConnectionString,
                DalDirectory = dalDirectory,
                TemplateDirectory = templateDirectory
            };
        }
    }
}