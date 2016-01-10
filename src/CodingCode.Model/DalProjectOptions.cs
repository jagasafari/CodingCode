namespace CodingCode.Model
{
    using Common.Core;
    public class DalProjectOptions
    {
        private string _templateDirectory;
        public string TemplateDirectory { get { return _templateDirectory; } set { _templateDirectory = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        private string _dalParentDirectory;
        public string DalParentDirectory { get { return _dalParentDirectory; } set { _dalParentDirectory = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
    }
}