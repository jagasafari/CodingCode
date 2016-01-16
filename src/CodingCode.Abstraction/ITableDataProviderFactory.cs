namespace CodingCode.Abstraction
{
    using System;

    public interface ITableDataProviderFactory
    {
        ITableDataProvider Create(dynamic databaseContext, Type entityType, int maxNumberOfRows);
    }
}