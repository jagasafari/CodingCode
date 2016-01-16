namespace CodingCode.Web.ViewComponents
{
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Mvc;
    public class JavascriptLinksViewComponent : ViewComponent
    {
        private bool _isDevelopment;
        public JavascriptLinksViewComponent(IHostingEnvironment hostingEnv)
        {
            _isDevelopment = hostingEnv.IsDevelopment();
        }
        
        public IViewComponentResult Invoke()
        {
            if (_isDevelopment) return View("Development");
            return View();
        }
    }
}