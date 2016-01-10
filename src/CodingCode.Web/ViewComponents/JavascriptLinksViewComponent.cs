namespace CodingCode.Web.ViewComponents
{
    using Microsoft.AspNet.Mvc;
    public class JavascriptLinksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}