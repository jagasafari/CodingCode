namespace CodingCode.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CodingCode.Abstraction;
    using CodingCode.ViewModel;
    using Common.Core;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;

    public class TableDataProvider : ITableDataProvider
    {
        private readonly Type _entityType;
        private readonly dynamic _databaseContext;
        private readonly ILogger<ITableDataProvider> _logger;
        private readonly int _maxNumberOfRows;
        public TableDataProvider(dynamic dbContext, Type entityType, ILogger<ITableDataProvider> logger, int maxNumberOfRows)
        {
            _entityType = entityType;
            _databaseContext = dbContext;
            _logger = logger;
            _maxNumberOfRows = maxNumberOfRows;
        }
        public string EntityTypeName => _entityType.Name;
        private string[] _columnNames;
        public string[] ColumnNames
        {
            get
            {
                if (_columnNames == null)
                {
                    PopulateColumnNames();
                }
                return _columnNames;
            }
            set { _columnNames = Check.NotNull(value); }
        }

        public TableViewModel MapToViewModel()
        {
            var tableValues = Check.NotNull(GetTableValues());

            return new TableViewModel
            {
                TableName = EntityTypeName,
                ColumnNames = ColumnNames,
                Values = tableValues
            };
        }

        private void PopulateColumnNames()
        {
            var propertyInfos = _entityType.GetTypeInfo().GetProperties();
            var columnPropertyInfos = propertyInfos.Where(FilterColumnPropertyName);

            ColumnNames = columnPropertyInfos.Select(x => x.Name).ToArray();
        }
        
        private bool FilterColumnPropertyName(PropertyInfo propertyInfo)
        {
            return NonNavTypesChecker.Check(propertyInfo.PropertyType) && propertyInfo.Name!="TypeName";
        }
        
        private string[,] GetTableValues()
        {
            var getValues = Check.NotNull(Check.NotNull(typeof(TableDataProvider).GetMethod(nameof(TableDataProvider.GetValues)))
                .MakeGenericMethod(_entityType));

            _logger.LogCritical("GetValues MethodInfo created");
            var values=Check.NotNull(getValues.Invoke(this, null));
            return (string[,])values;
        }

        public string[,] GetValues<T>() where T : class
        {
            _logger.LogCritical("getting table from context");
            var dbSet = Check.NotNull(_databaseContext.GetTable<T>());
            T[] table = Check.NotNull(((DbSet<T>)dbSet).Take(_maxNumberOfRows).ToArray());

            var tableValues = new string[table.Length, ColumnNames.Length];

            var populateDictionary = _entityType.GetMethod("PopulateDictionary");
            _logger.LogCritical($"starting iterate rows");
            for (var i = 0; i < table.Length; i++)
            {
                var rowValues = Check.NotNull((Dictionary<string, dynamic>)populateDictionary.Invoke(table[i], null));
                for (var j = 0; j < ColumnNames.Length; j++)
                {
                    if (!rowValues.ContainsKey(ColumnNames[j]))
                        throw new Exception($"Row dictionary of {EntityTypeName} does not contain {ColumnNames[j]} key");
                    var nextColumnValue = rowValues[ColumnNames[j]];
                    tableValues[i, j] = (nextColumnValue ?? string.Empty).ToString();
                }
            }

            return tableValues;
        }
    }
}