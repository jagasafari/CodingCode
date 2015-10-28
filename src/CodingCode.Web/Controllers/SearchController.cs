// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CodingCode.Web.Controllers
{
    using Contracts;
    using Logic;
    using Microsoft.AspNet.Mvc;
    using ViewModels;

    public class SearchController : Controller
    {
        private readonly ICodeFounder _codeFounder;

        public SearchController(ICodeFounder codeFounder)
        {
            _codeFounder = codeFounder;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetFileCode(string fullPath)
        {
            var fileContent = CodeFounder.GetFileContent(fullPath);
            return PartialView(fileContent);
        }

        [HttpPost]
        public IActionResult MatchingFiles(SearchedCodeViewModel model)
        {
            var machingFiles = _codeFounder.GetMachingFiles(model);

            var machingFilesViewModel = new MachingFilesViewModel
            {
                MachingFiles = machingFiles,
                FirstFileContent =
                    machingFiles.Length > 0
                        ? CodeFounder.GetFileContent(machingFiles[0])
                        : null
            };

            return View(machingFilesViewModel);
        }
    }
}