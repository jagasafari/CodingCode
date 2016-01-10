namespace CodingCode.Web.Controllers
{
    using System;
    using System.Reflection;
    using CodingCode.Abstraction;
    using Common.Core;
    using Microsoft.AspNet.Mvc;
    using Services;
    using ViewModel;

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
                [FromServices] IQueryRequestMapper queryRequestMapper,
                [FromServices] DbContextWrapper dbContextWrapper)
        {
            Type randomType = randomTablePicker.GetRandomTable(dbContextWrapper[assemblyName]);
            
            MethodInfo generic = typeof(IQueryRequestMapper).GetMethod(nameof(IQueryRequestMapper.MapToViewModel)).MakeGenericMethod(randomType);
            TableViewModel mapToViewModel = (TableViewModel)generic.Invoke(queryRequestMapper, new object[] { dbContextWrapper[assemblyName] });

            return View(mapToViewModel);
        }
    }
}