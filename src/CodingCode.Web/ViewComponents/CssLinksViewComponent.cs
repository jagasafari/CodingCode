using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;

namespace CodingCode.Web.ViewComponents{
   public class CssLinksViewComponent:ViewComponent{
       private bool _isDevelopment;
       public CssLinksViewComponent(IHostingEnvironment hostingEnv){
          _isDevelopment = hostingEnv.IsDevelopment();
       }
       
        public IViewComponentResult Invoke()
        {
            if (_isDevelopment) return View("Development");
            return View();
        }
   }
}