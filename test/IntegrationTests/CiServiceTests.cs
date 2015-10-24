namespace IntegrationTests
{
    using System.IO;
    using CIService;
    using Xunit;

    public class CiServiceTests
    {
        [Fact]
        public void CountFiles_RootFilesLessEqualRecurseFileCount()
        {
            var path = @"C:\Users\mika\Documents\Visual Studio 2015\Projects\CodingCode";
            var filesToTest = Program.GetFiles(path).Length;
            var c1 = new DirectoryInfo(path).GetFiles().Length;
            var c2 = new DirectoryInfo(path).GetFiles("*.*",SearchOption.AllDirectories).Length;
            Assert.True(c1<c2);
            Assert.Equal(filesToTest,c2);
        }

        [Fact]
        public void ConfigFile_ReadData_SectionAsStringArray()
        {
            var ciTestConfiguration = CiTestConfigurationReader.GetConfiguration();
            Assert.Equal(3,ciTestConfiguration.TestProjects.Count);
        }
    }
}