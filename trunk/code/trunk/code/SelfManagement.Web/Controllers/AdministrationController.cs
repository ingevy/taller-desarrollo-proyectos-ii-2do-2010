namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Web.ViewModels;

    public class AdministrationController : Controller
    {
        public ActionResult Index()
        {
            return this.View("Users");
        }

        public ActionResult Users()
        {
            return this.View();
        }

        public ActionResult FileLogs()
        {
            return this.View(new FileFilterViewModel());
        }
    }
}
