using System.Web.Mvc;

namespace WorkProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Login Page";

            return View();
        }

     
    }
}
