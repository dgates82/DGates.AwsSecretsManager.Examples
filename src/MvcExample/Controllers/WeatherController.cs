using System.Web.Mvc;

namespace MvcExample.Controllers
{
    public class WeatherController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
