using CodingCode.ViewModel;

namespace CodingCode.Abstraction
{
    public interface ITableDataProvider
    {
        string EntityTypeName { get; }
        string[] ColumnNames { get; set; }
        TableViewModel MapToViewModel();
    }
}