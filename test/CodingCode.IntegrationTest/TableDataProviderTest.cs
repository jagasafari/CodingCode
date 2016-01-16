namespace CodingCode.IntegrationTest
{
    using System;
    using CodingCode.Abstraction;
    using CodingCode.Model;
    using CodingCode.Services;
    using CodingCode.ViewModel;
    using Common.ProcessExecution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using System.Linq;
    using System.IO;

    public class TableDataProviderTest
    {
        private readonly IServiceProvider _serviceProvider;

        public TableDataProviderTest()
        {
            var configurations = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
            var serviceCollection = new ServiceCollection()
                 .AddLogging()
                 .AddOptions()
                 .Configure<DnxOptions>(configurations)
                 .Configure<DalProjectOptions>(configurations)
                 .AddProceesProviderServices()
                 .AddTransient<IDataAccessSettingsMapper, DataAccessSettingsMapper>()
                 .AddTransient<IDalGeneratorFactory, DalGeneratorFactory>()
                 .AddTransient<IContextGenerator, ContextGenerator>()
                 .AddTransient<ITableDataProviderFactory, TableDataProviderFactory>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceProvider.GetService<ILoggerFactory>().AddConsole().MinimumLevel = LogLevel.Debug;
        }

        [Fact]
        public async void MapRaportRequestToViewModel_NorthwindContextOrdersType_NumberOfColumnsInOrdersTable()
        {
            var assemblyName = "TestAssembly1";
            var viewModel = new DataAccessViewModel { ServerName = @"DELL\SQLEXPRESS", DatabaseName = "Northwind" };
            var dbContext = await _serviceProvider.GetService<IContextGenerator>().GenerateAsync(viewModel, assemblyName);
            var ordersType = dbContext.GetType().Assembly.GetTypes().Single(type => type.Name == "Orders");
            var factory = _serviceProvider.GetService<ITableDataProviderFactory>();
            var tableDataProvider = factory.Create(dbContext, ordersType, 50);
            Assert.NotNull(tableDataProvider);
            var columnNames = tableDataProvider.ColumnNames;

            Assert.NotNull(columnNames);
            int expectedNumberOfColumns = 14;
            Assert.Equal(expectedNumberOfColumns, columnNames.Length);
        }

        [Fact]
        public async void MapRaportRequestToViewModel_NorthwindContextOrdersType_ColumnNamesInOrdersTable()
        {
            var assemblyName = "TestAssembly2";
            var viewModel = new DataAccessViewModel { ServerName = @"DELL\SQLEXPRESS", DatabaseName = "Northwind" };
            var dbContext = await _serviceProvider.GetService<IContextGenerator>().GenerateAsync(viewModel, assemblyName);
            var ordersType = dbContext.GetType().Assembly.GetTypes().Single(type => type.Name == "Orders");
            var tableDataProvider = _serviceProvider.GetService<ITableDataProviderFactory>().Create(dbContext, ordersType, 50);
            var columnNames = tableDataProvider.ColumnNames;
            var expectedColumnNames = new[] { "OrderID", "CustomerID", "EmployeeID", "Freight", "OrderDate", "RequiredDate", "ShipAddress",
                "ShipCity", "ShipCountry", "ShipName", "ShippedDate", "ShipPostalCode", "ShipRegion", "ShipVia" };
            var logger = _serviceProvider.GetService<ILogger<TableDataProviderTest>>();
            for (var i = 0; i < expectedColumnNames.Length; i++)
            {
                logger.LogCritical($"expecting {expectedColumnNames[i]} in column names");
                columnNames.Single(x => x == expectedColumnNames[i]);
            }
        }

        [Fact]
        public async void MapRaportRequestToViewModel_NorthwindContextOrdersType_ColumnNamesHasOnlyExpectedValues()
        {
            var assemblyName = "TestAssembly3";
            var viewModel = new DataAccessViewModel { ServerName = @"DELL\SQLEXPRESS", DatabaseName = "Northwind" };
            var dbContext = await _serviceProvider.GetService<IContextGenerator>().GenerateAsync(viewModel, assemblyName);
            var ordersType = dbContext.GetType().Assembly.GetTypes().Single(type => type.Name == "Orders");
            var tableDataProvider = _serviceProvider.GetService<ITableDataProviderFactory>().Create(dbContext, ordersType, 50);
            var columnNames = tableDataProvider.ColumnNames;
            var expectedColumnNames = new[] { "OrderID", "CustomerID", "EmployeeID", "Freight", "OrderDate", "RequiredDate", "ShipAddress",
                "ShipCity", "ShipCountry", "ShipName", "ShippedDate", "ShipPostalCode", "ShipRegion", "ShipVia" };

            var logger = _serviceProvider.GetService<ILogger<TableDataProviderTest>>();
            for (var i = 0; i < columnNames.Length; i++)
            {
                logger.LogCritical($"Asserting {columnNames[i]} is expected columnName");
                expectedColumnNames.Single(x => x == columnNames[i]);
            }
        }
        [Fact]
        public async void MapRaportRequestToViewModel_NorthwindContextOrdersType_TableViewModelValuesPopulated()
        {
            var assemblyName = "TestAssembly4";
            var viewModel = new DataAccessViewModel { ServerName = @"DELL\SQLEXPRESS", DatabaseName = "Northwind" };
            var dbContext = await _serviceProvider.GetService<IContextGenerator>().GenerateAsync(viewModel, assemblyName);
            var ordersType = dbContext.GetType().Assembly.GetTypes().Single(type => type.Name == "Orders");
            var tableDataProvider = _serviceProvider.GetService<ITableDataProviderFactory>().Create(dbContext, ordersType, 50);
            var tableViewModel = tableDataProvider.MapToViewModel();
            Assert.NotNull(tableViewModel.Values);
        }

        [Fact]
        public async void GenrateDalProjectFromExistingDatabase_Northwind_NumberOfGEneratedFilesAndFileNames()
        {
            var generator = _serviceProvider.GetService<IContextGenerator>();

            var assemblyName = "ContextGenratorTest_TestAssembly";
            var viewModel = new DataAccessViewModel() { DatabaseName = "Northwind", ServerName = @"DELL\SQLEXPRESS" };
            var objectInstance = await generator.GenerateAsync(viewModel, assemblyName);
            Assert.NotNull(objectInstance);

            var testAssembly = $@"C:\MyGit\CodingCode\test\TestOutput\{assemblyName}";
            Assert.True(Directory.Exists(testAssembly));

            var files = Directory.GetFiles(testAssembly).Select(path => Path.GetFileName(path)).ToArray();
            var expectedNumberofFiles = 16;
            Assert.Equal(expectedNumberofFiles, files.Length);

            var expectedFilesListPath = @"C:\MyGit\CodingCode\test\CodingCode.IntegrationTest\TestData\TestAssemblyExpectedFiles.txt";
            var expectedFilesList = File.ReadAllLines(expectedFilesListPath).ToList();
            var logger = _serviceProvider.GetService<ILogger<TableDataProviderTest>>();
            for (var i = 0; i < files.Length; i++)
            {
                logger.LogCritical($"asserting that expected file list contains {files[i]}, generated by contextGenerator");
                Assert.True(expectedFilesList.Contains(files[i]));
            }
        }
    }
}