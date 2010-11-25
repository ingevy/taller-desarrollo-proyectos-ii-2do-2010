namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;

    public class AdministrationController : Controller
    {
        private readonly IMetricsRepository metricsRepository;

        public AdministrationController()
            : this(new RepositoryFactory().GetMetricsRepository())
        {
        }

        public AdministrationController(IMetricsRepository metricsRepository)
        {
            this.metricsRepository = metricsRepository;
        }

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

        [Authorize(Roles = "ITManager")]
        public ActionResult Files(string dataDate, string processingDate, string modifiedDate, int? type, int? state)
        {
            var files = this.metricsRepository.FilterProcessedFiles(dataDate, processingDate, modifiedDate, type, state);

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = files };
        }
    }
}
