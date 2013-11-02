using System.Web.Mvc;

namespace Uptime_Jalgpall.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is football application. It uses following features:";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Jevgeni's contact page.";

            return View();
        }
    }
}