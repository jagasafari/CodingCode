namespace CodingCode.Web.Controllers
{
    using Contracts;
    using Microsoft.AspNet.Mvc;
    using ViewModels;

    public class SearchController : Controller
    {
        private readonly ICodeFounderFactory _codeFounderFactory;

        public SearchController(ICodeFounderFactory codeFounderFactory)
        {
            _codeFounderFactory = codeFounderFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult FileCode(string path)
        {
            return Json(System.IO.File.ReadAllLines(path));
        }

        [HttpPost]
        public IActionResult MatchingFiles(SearchedCodeViewModel model)
        {
            var codeFounder = _codeFounderFactory.Create(model);
            var machingFiles = codeFounder.GetMachingFiles();

            var machingFilesViewModel = new MachingFilesViewModel
            {
                MachingFiles = machingFiles,
                FirstFileContent =
                    machingFiles.Length > 0
                        ? System.IO.File.ReadAllLines(machingFiles[0])
                        : null
            };

            return View(machingFilesViewModel);
        }
    }
}