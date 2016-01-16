namespace CodingCode.ViewModel
{
    using Common.Core;
    public class TableViewModel
    {
        private string[] _columnNames;
        private string _tableName;
        private string[,] _values;
        public string[] ColumnNames { get { return _columnNames; } set { _columnNames = Check.NotNull(value, nameof(value)); } }
        public string TableName { get { return _tableName; } set { _tableName = Check.NotNullOrWhiteSpace(value, nameof(value)); } }
        public string[,] Values { get { return _values; } set { _values = Check.NotNull(value, nameof(value)); } }
    }
}