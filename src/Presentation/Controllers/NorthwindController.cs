namespace Presentation.Controllers
{
    using System;
    using Contracts;
    using Logic;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Dnx.Runtime;
    using Model;

    public class NorthwindController : Controller
    {
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly IDalGeneratorFactory _dalGeneratorFactory;
        private readonly DbContextWrapper _dbContextWrapper;
        private readonly IQueryRequestMapper _queryRequestMapper;
        private readonly IRandomTablePicker _randomTablePicker;


        public NorthwindController(
            DbContextWrapper dbContextWrapper,
            IQueryRequestMapper queryRequestMapper,
            IRandomTablePicker randomTablePicker,
            IApplicationEnvironment applicationEnvironment,
            IDalGeneratorFactory dalGeneratorFactory)
        {
            _dbContextWrapper = dbContextWrapper;
            _queryRequestMapper = queryRequestMapper;
            _randomTablePicker = randomTablePicker;
            _applicationEnvironment = applicationEnvironment;
            _dalGeneratorFactory = dalGeneratorFactory;
        }

        public IActionResult Index()
        {
            var connection =
                @"""Server=DELL\SQLEXPRESS;Database=Northwind;Trusted_Connection=True;MultipleActiveResultSets=true""";
            var assemblyName =
                DalGenerator.GenerateAssemblyName(connection);

            if(! _dbContextWrapper.Exists(assemblyName))
            {
                var dalInfo = new DalInfo
                {
                    ConnectionString = connection,
                    AssemblyName = assemblyName,
                    DatabaseName = "Northwind",
                    AssemblyBasePath =
                        _applicationEnvironment.ApplicationBasePath
                };
                _dalGeneratorFactory.DalInfo = dalInfo;

                using(
                    var dalGenerator = _dalGeneratorFactory.Create())
                {
                    dalGenerator.CreateDalDirectory();

                    dalGenerator.CopyProjectJson();

                    dalGenerator.Restore();

                    dalGenerator.Scaffold();

                    dalGenerator.CodeContext();

                    dalGenerator.CodeEntities();

                    dalGenerator.Build();

                    _dbContextWrapper[assemblyName] =
                        dalGenerator.InstantiateDbContext();
                }
            }

            Type randomType =
                _randomTablePicker.GetRandomTable(
                    _dbContextWrapper[assemblyName]);

            TableViewModel mapToViewModel =
                _queryRequestMapper.MapToViewModel(randomType,
                    _dbContextWrapper[assemblyName]);

            return View(mapToViewModel);
        }
    }
}