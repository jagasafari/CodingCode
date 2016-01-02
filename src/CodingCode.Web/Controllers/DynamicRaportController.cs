namespace CodingCode.Web.Controllers
{
    using System;
    
    using Contracts;
    using Microsoft.AspNet.Mvc;
    using Services;
    using ViewModel;

    public class DynamicRaportController : Controller
    {
        public IActionResult Index(string assemblyName){
            if(string.IsNullOrWhiteSpace(assemblyName)){
                return View("Error");
            }
            ViewData["assemblyName"]=assemblyName;
            return View();
        }
        
        [HttpGet]
        public IActionResult RandomTable(string assemblyName, 
                [FromServices] IQueryRequestMapper queryRequestMapper,
                [FromServices] IRandomTablePicker randomTablePicker,
                [FromServices] DbContextWrapper dbContextWrapper)
        {
            Type randomType =
                randomTablePicker.GetRandomTable(
                    dbContextWrapper[assemblyName]);

            TableViewModel mapToViewModel =
                queryRequestMapper.MapToViewModel(randomType,
                    dbContextWrapper[assemblyName]);

            return View(mapToViewModel);
        }
    }
}