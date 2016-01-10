namespace CodingCode.Web.Controllers
{
    using System.Threading.Tasks;
    using CodingCode.Abstraction;
    using CodingCode.Services;
    using CodingCode.ViewModel;
    using Microsoft.AspNet.Mvc;

    public class DataAccessScaffoldController : Controller
    {
        [HttpGet]
        public IActionResult CodeDatabase() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CodeDatabase(DataAccessViewModel dataAccessViewModel,
            [FromServices] DbContextWrapper dbContextWrapper, [FromServices] IContextGenerator contextGenerator)
        {
            if (!ModelState.IsValid) return View(dataAccessViewModel);

            string assemblyName = ComposeAssemblyName(dataAccessViewModel);
            
            if (!dbContextWrapper.Exists(assemblyName))
                dbContextWrapper[assemblyName] = await contextGenerator.GenerateAsync(dataAccessViewModel, assemblyName);
                
            return RedirectToAction(nameof(DynamicRaportController.Index), "DynamicRaport", new { assemblyName = assemblyName });
        }
        
        private static string ComposeAssemblyName(DataAccessViewModel dataAccessViewModel){
          return string.Concat(dataAccessViewModel.ServerName.Replace("\\", "_"), dataAccessViewModel.DatabaseName);
        }
    }
}