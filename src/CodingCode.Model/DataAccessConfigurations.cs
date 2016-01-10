using Common.Core;

namespace CodingCode.Model
{
    public class DataAccessConfigurations
    {
        private string _databaseName;
        private string _serverName;
        private string _assemblyName;
        private string _projectDirectory;
        private string _templateDirectory;
        public string DatabaseName { get { return _databaseName; } set { _databaseName = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string ServerName { get { return _serverName; } set { _serverName = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string AssemblyName { get { return _assemblyName; } set { _assemblyName = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string ProjectDirectory { get { return _projectDirectory; } set { _projectDirectory = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string TemplateDirectory { get { return _templateDirectory; } set { _templateDirectory = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
    }
}