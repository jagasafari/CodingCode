namespace Presentation.Model
{
    public class TableViewModel
    {
        public string[] ColumnNames { get; set; }

        public string TableName { get; set; }

        public string[,] Values { get; set; }
    }
}