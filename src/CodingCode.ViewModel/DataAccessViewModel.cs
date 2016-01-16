namespace CodingCode.ViewModel
{
    using Common.Core;
    public class 
    DataAccessViewModel
    {
        private string _serverName;
        private string _databaseName;
        public string ServerName { get { return _serverName; } set { _serverName = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string DatabaseName { get { return _databaseName; } set { _databaseName = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
    }
}