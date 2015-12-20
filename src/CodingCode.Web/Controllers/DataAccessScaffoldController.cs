namespace CodingCode.Web.Controllers
{
    using System.Threading.Tasks;
    using CodingCode.Contracts;
    using CodingCode.Services;
    using CodingCode.ViewModel;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;

    public class DataAccessScaffoldController : Controller
    {
        private readonly DbContextWrapper _dbContextWrapper;
        private readonly IContextGenerator _contextGenerator;
        private readonly ILogger _logger;

        public DataAccessScaffoldController(ProviderServices providerServices)
        {
            _dbContextWrapper = providerServices.DbContextWrapper;
            _contextGenerator = providerServices.ContextGenerator;
            _logger = providerServices.ConsoleLogger(nameof(DataAccessScaffoldController));
        }

        public IActionResult CodeDatabase()
        {
            _logger.LogInformation("coding data access layer");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CodeDatabase(
            DataAccessViewModel dataAccessViewModel)
        {
            if (ModelState.IsValid)
            {
                string assemblyName = string.Concat(dataAccessViewModel.ServerName.Replace("\\", "_"), dataAccessViewModel.DatabaseName);

                if (!_dbContextWrapper.Exists(assemblyName))
                {
                    _dbContextWrapper[assemblyName] =
                        await
                            _contextGenerator.GenerateAsync(dataAccessViewModel, assemblyName);
                }

                return RedirectToAction(nameof(DynamicRaportController.Index), "DynamicRaport", new {assemblyName = assemblyName});
            }

            return View(dataAccessViewModel);
        }
        
    }
}