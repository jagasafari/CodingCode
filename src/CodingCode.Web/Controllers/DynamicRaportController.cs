namespace CodingCode.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Logic;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Dnx.Runtime;
    using ViewModel;

    public class DynamicRaportController : Controller
    {
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly IDalGeneratorFactory _dalGeneratorFactory;
        private readonly DbContextWrapper _dbContextWrapper;
        private readonly IQueryRequestMapper _queryRequestMapper;
        private readonly IRandomTablePicker _randomTablePicker;

        public DynamicRaportController(
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CodeDatabaseModel(DalInfoViewModel dalInfo)
        {
            var assemblyName =
                DalGenerator.GenerateAssemblyName(dalInfo.ConnectionString);
            dalInfo.AssemblyName = assemblyName;

            if (!_dbContextWrapper.Exists(assemblyName))
            {
                dalInfo.AssemblyBasePath =
                    _applicationEnvironment.ApplicationBasePath;
                dalInfo.DatabaseName = "Northwind";
                _dalGeneratorFactory.DalInfoViewModel = dalInfo;

                using (
                    var dalGenerator = _dalGeneratorFactory.Create())
                {
                    dalGenerator.CreateDalDirectory();

                    dalGenerator.CopyProjectJson();

                    await dalGenerator.RestoreAsync();

                    await dalGenerator.ScaffoldAsync();

                    dalGenerator.CodeContext();

                    await dalGenerator.CodeEntitiesAsync();

                    await dalGenerator.BuildAsync();

                    _dbContextWrapper[assemblyName] =
                        dalGenerator.InstantiateDbContext();
                }
            }
            return View(dalInfo);
        }

        [HttpGet, ActionName("RandomTable")]
        public IActionResult RandomTable(string assemblyName)
        {
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