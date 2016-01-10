using Common.Core;

namespace CodingCode.ViewModel
{
    public class DataAccessViewModel
    {
        private string _serverName;
        public string ServerName {get{ return _serverName;} set { _serverName=Check.NotNullOrWhiteSpace(value,nameof(value));}}
        private string _databaseName;
        public string DatabaseName {get{ return _databaseName;} set { _databaseName=Check.NotNullOrWhiteSpace(value,nameof(value));}}
    }
}