namespace CodingCode.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Logic;
    using Microsoft.AspNet.Mvc;
    using ViewModels;

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
        public async Task<IActionResult> CodeDatabaseModel(
            DalInfoViewModel dalInfo)
        {
            var assemblyName =
                string.Concat(dalInfo.Server.Replace("\\", "_"),
                    dalInfo.Database);

            if(_dbContextWrapper.Exists(assemblyName))
                return View((object)assemblyName);

            _dbContextWrapper[assemblyName] =
                await
                    _contextGenerator.GenerateAsync(dalInfo, assemblyName);

            return View((object)assemblyName);
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