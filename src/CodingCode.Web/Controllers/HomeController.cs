namespace CodingCode.Web.Controllers
{
    using Microsoft.AspNet.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            //log to console
            //log to file
            //log errors to database
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}