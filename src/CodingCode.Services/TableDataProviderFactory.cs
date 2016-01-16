namespace CodingCode.Services{
    using System;
    using CodingCode.Abstraction;
    using Microsoft.Extensions.Logging;

    public class TableDataProviderFactory : ITableDataProviderFactory
    {
        private readonly ILogger<ITableDataProvider> _logger;
        public TableDataProviderFactory(ILogger<ITableDataProvider> logger){
           _logger = logger;
        }
       public ITableDataProvider Create(dynamic databaseContext, Type entityType, int maxNumberOfRows) 
       {
           return new TableDataProvider(databaseContext, entityType, _logger, maxNumberOfRows);
       }
   }
}