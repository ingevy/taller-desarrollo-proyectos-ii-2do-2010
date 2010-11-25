namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;
    using System.Globalization;
    using System;
    using System.IO;

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

            return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = files.Select(f => new
                        {
                            Id = f.Id,
                            Path = f.FileSystemPath,
                            FileType = ((ExternalSystemFiles)f.FileType).ToString(),
                            CssClass = f.HasErrors ? "file_with_errors" : "file_no_errors",
                            State = f.HasErrors ? "Con Errores" : "Sin Errores",
                            DateData = GetDateDataFromFile(f),
                            DateProcessed = f.DateProcessed.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                            DateLastModified = f.DateLastModified.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                        })
                };
        }

        private static string GetDateDataFromFile(ProcessedFile file)
        {
            if (((ExternalSystemFiles)file.FileType) == ExternalSystemFiles.HF)
            {
                return "N/A";
            }

            return DateTime
                        .ParseExact(Path.GetFileNameWithoutExtension(file.FileSystemPath).Split('_')[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)
                        .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
