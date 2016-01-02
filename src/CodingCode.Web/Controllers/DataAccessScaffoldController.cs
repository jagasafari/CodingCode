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
        private readonly IContextGenerator _contextGenerator;
        private readonly ILogger _logger;

        public DataAccessScaffoldController(ProviderServices providerServices,
            ILogger<DataAccessScaffoldController> logger)
        {
            _logger = logger;
        }

        public IActionResult CodeDatabase()
        {
            _logger.LogInformation("coding data access layer");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CodeDatabase(
            DataAccessViewModel dataAccessViewModel,
            [FromServices] DbContextWrapper dbContextWrapper,
            [FromServices] IContextGenerator contextGenerator)
        {
            if (ModelState.IsValid)
            {
                string assemblyName = string.Concat(dataAccessViewModel.ServerName.Replace("\\", "_"), dataAccessViewModel.DatabaseName);

                if (!dbContextWrapper.Exists(assemblyName))
                {
                    dbContextWrapper[assemblyName] =
                        await
                            contextGenerator.GenerateAsync(dataAccessViewModel, assemblyName);
                }

                return RedirectToAction(nameof(DynamicRaportController.Index), "DynamicRaport", new {assemblyName = assemblyName});
            }

            return View(dataAccessViewModel);
        }
        
    }
}