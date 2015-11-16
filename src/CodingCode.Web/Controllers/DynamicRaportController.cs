namespace CodingCode.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNet.Mvc;
    using Services;
    using ViewModel;

    public class DynamicRaportController : Controller
    {
        private readonly IContextGenerator _contextGenerator;
        private readonly DbContextWrapper _dbContextWrapper;
        private readonly IQueryRequestMapper _queryRequestMapper;
        private readonly IRandomTablePicker _randomTablePicker;

        public DynamicRaportController(
            DbContextWrapper dbContextWrapper,
            IQueryRequestMapper queryRequestMapper,
            IRandomTablePicker randomTablePicker,
            IContextGenerator contextGenerator)
        {
            _dbContextWrapper = dbContextWrapper;
            _queryRequestMapper = queryRequestMapper;
            _randomTablePicker = randomTablePicker;
            _contextGenerator = contextGenerator;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CodeDatabase(
            DalInfoViewModel dalInfo)
        {
            var databaseCodedViewModel = new DatabaseCodedViewModel()
            {
                AssemblyName =
                    string.Concat(dalInfo.Server.Replace("\\",
                        "_"),
                        dalInfo.Database)
            };

            if(_dbContextWrapper.Exists(databaseCodedViewModel.AssemblyName))
                return View(databaseCodedViewModel);

            _dbContextWrapper[databaseCodedViewModel.AssemblyName] =
                await
                    _contextGenerator.GenerateAsync(dalInfo, databaseCodedViewModel.AssemblyName);

            return View(databaseCodedViewModel);
        }


        [HttpGet, ActionName("RandomTable")]
        public IActionResult RandomTable(DatabaseCodedViewModel databaseCoded)
        {
            Type randomType =
                _randomTablePicker.GetRandomTable(
                    _dbContextWrapper[databaseCoded.AssemblyName]);

            TableViewModel mapToViewModel =
                _queryRequestMapper.MapToViewModel(randomType,
                    _dbContextWrapper[databaseCoded.AssemblyName]);

            return View(mapToViewModel);
        }
    }
}