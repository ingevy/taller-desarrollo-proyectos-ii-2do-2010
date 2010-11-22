namespace CallCenter.SelfManagement.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class AdministrationController : Controller
    {
        public ActionResult Index()
        {
            return View("Users");
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult FileLogs()
        {
            return View();
        }
    }
}
