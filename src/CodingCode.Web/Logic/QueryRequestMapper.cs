namespace CodingCode.Web.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Contracts;
    using ViewModels;

    public class QueryRequestMapper : IQueryRequestMapper
    {
        public TableViewModel MapToViewModel(Type randomType,
            dynamic dbContext)
        {
            var colNames = GetColumnNames(randomType);
            var tableValues = GetValues(randomType, colNames, dbContext);

            return new TableViewModel
            {
                TableName = randomType.Name,
                ColumnNames = colNames,
                Values = tableValues
            };
        }

        private static string[] GetColumnNames(Type randomType)
        {
            var propertyInfos = randomType.GetTypeInfo().GetProperties();
            var colNames = ( from propertyInfo in propertyInfos
                where NonNavTypesChecker.Check(propertyInfo.PropertyType)
                select propertyInfo.Name ).ToArray();
            return colNames;
        }

        private string[,] GetValues(Type randomType,
            IReadOnlyList<string> colNames, dynamic dbContext)
        {
            var table = dbContext.GetTable(randomType.ToString());
            var ts = table.Length;
            var numRows = ts > 50 ? 50 : ts;
            var tableValues = new string[numRows, colNames.Count];
            for(var i = 0; i < numRows; i++)
            {
                table[i].PopulateDictionary();
                for(var j = 0; j < colNames.Count; j++)
                {
                    dynamic o = table[i].Dictionary[colNames[j]];
                    tableValues[i, j] = o == null
                        ? ""
                        : string.Format("{0}", o);
                }
            }
            return tableValues;
        }
    }
}