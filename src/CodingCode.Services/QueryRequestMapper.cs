namespace CodingCode.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CodingCode.Abstraction;
    using Microsoft.Data.Entity;
    using ViewModel;

    public class QueryRequestMapper : IQueryRequestMapper
    {
        public TableViewModel MapToViewModel<T>(dynamic dbContext) where T : class
        {
            var colNames = GetColumnNames<T>();
            var tableValues = GetValues<T>(colNames, dbContext);

            return new TableViewModel
            {
                TableName = typeof(T).Name,
                ColumnNames = colNames,
                Values = tableValues
            };
        }

        private static string[] GetColumnNames<T>()
        {
            var propertyInfos = typeof(T).GetTypeInfo().GetProperties();

            return (from propertyInfo in propertyInfos where NonNavTypesChecker.Check(propertyInfo.PropertyType) select propertyInfo.Name).ToArray();
        }

        private string[,] GetValues<T>(IReadOnlyList<string> columnNames, dynamic dbContext) where T : class
        {
            var maxNumberOfRows = 50;
            T[] table = ((DbSet<T>)dbContext.GetTable<T>()).Take(maxNumberOfRows).ToArray();

            var tableValues = new string[table.Length, columnNames.Count];

            MethodInfo method = typeof(T).GetMethod("PopulateDictionary");
            for (var i = 0; i < table.Length; i++)
            {
                var rowValues = (Dictionary<string, dynamic>)method.Invoke(table[i], null);
                for (var j = 0; j < columnNames.Count; j++)
                    tableValues[i, j] = (rowValues[columnNames[j]] ?? string.Empty).ToString();
            }

            return tableValues;
        }
    }
}