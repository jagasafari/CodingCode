namespace CodingCode.Web.Controllers
{
    using System;
    
    using Contracts;
    using Microsoft.AspNet.Mvc;
    using Services;
    using ViewModel;

    public class DynamicRaportController : Controller
    {
        private readonly DbContextWrapper _dbContextWrapper;
        private readonly IQueryRequestMapper _queryRequestMapper;
        private readonly IRandomTablePicker _randomTablePicker;

        public DynamicRaportController(
            ProviderServices providerServices)
        {
            _dbContextWrapper = providerServices.DbContextWrapper;
            _queryRequestMapper = providerServices.QueryRequestMapper;
            _randomTablePicker = providerServices.RandomTablePicker;
        }
        
        public IActionResult Index(string assemblyName){
            if(string.IsNullOrWhiteSpace(assemblyName)){
                return View("Error");
            }
            ViewData["assemblyName"]=assemblyName;
            return View();
        }
        
        [HttpGet]
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