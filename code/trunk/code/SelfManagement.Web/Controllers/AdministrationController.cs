namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Web.ViewModels;

    public class AdministrationController : Controller
    {
        [Authorize(Roles = "ITManager")]
        public ActionResult Index()
        {
            return this.View("Users");
        }

        [Authorize(Roles = "ITManager")]
        public ActionResult Users()
        {
            return this.View();
        }

        [Authorize(Roles = "ITManager")]
        public ActionResult FileLogs()
        {
            return this.View(new FileFilterViewModel());
        }
    }
}
