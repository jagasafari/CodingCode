﻿namespace CodingCode.Web.Controllers
{
    using Microsoft.AspNet.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Error() => View("~/Views/Shared/Error.cshtml");
    }
}