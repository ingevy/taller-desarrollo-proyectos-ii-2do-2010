namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;

    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            this.ViewData["WelcomeMessage"] = "¡Bienvenido al sistema SelfManagement!";

            return View();
        }
    }
}
