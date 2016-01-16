namespace CodingCode.Web.Controllers
{
    using System;
    using CodingCode.Abstraction;
    using Common.Core;
    using Microsoft.AspNet.Mvc;
    using Services;

    public class DynamicRaportController : Controller
    {
        [HttpGet]
        public IActionResult Index(string assemblyName)
        {
            try
            {
                ViewData[nameof(assemblyName)] = Check.NotNullOrWhiteSpace(assemblyName, nameof(assemblyName));
            }
            catch (Exception)
            {
                return View(nameof(HomeController.Error));
            }
            return View();
        }

        [HttpGet]
        public IActionResult RandomTable(string assemblyName,
                [FromServices] IRandomTablePicker randomTablePicker,
                [FromServices] ITableDataProviderFactory tableDataProviderFactoy,
                [FromServices] DatabaseContextWrapper dbContextWrapper)
        {
            var randomType = randomTablePicker.GetRandomTable(dbContextWrapper[assemblyName]);
            var maxNumberOfRows=50;
            var tableProvider = tableDataProviderFactoy.Create(dbContextWrapper[assemblyName], randomType, maxNumberOfRows);
            return View(tableProvider.MapToViewModel());
        }
    }
}