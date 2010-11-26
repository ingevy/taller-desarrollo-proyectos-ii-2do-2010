namespace CallCenter.SelfManagement.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;
    using System.Web.Security;

    public class AdministrationController : Controller
    {
        private const NumberStyles NumberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number | NumberStyles.Currency;

        private readonly IMetricsRepository metricsRepository;
        private readonly IMembershipService membershipService;

        public AdministrationController()
            : this(new RepositoryFactory().GetMetricsRepository(), new RepositoryFactory().GetMembershipService())
        {
        }

        public AdministrationController(IMetricsRepository metricsRepository, IMembershipService membershipService)
        {
            this.metricsRepository = metricsRepository;
            this.membershipService = membershipService;
        }

        [Authorize(Roles = "ITManager")]
        public ActionResult Index()
        {
            return this.RedirectToAction("CreateUser");
        }

        [Authorize(Roles = "ITManager")]
        public ActionResult CreateUser()
        {
            return this.View(new UserViewModel { Role = 1 });
        }

        [HttpPost]
        [Authorize(Roles = "ITManager")]
        public ActionResult CreateUser(UserViewModel userToCreate)
        {
            var globalError = string.Empty;
            if (this.ModelState.IsValid)
            {
                MembershipCreateStatus status;
                if (string.IsNullOrWhiteSpace(userToCreate.Id))
                {
                    status = this.membershipService.CreateUser(userToCreate.Username, userToCreate.Password, userToCreate.Email);
                    if (status == MembershipCreateStatus.Success)
                    {
                        this.AddRoleAndProfile(userToCreate);

                        var innerUserId = this.membershipService.RetrieveInnerUserIdByUserName(userToCreate.Username);
                        return this.RedirectToAction("CreateUser", new { msg = Server.UrlEncode(string.Format(CultureInfo.InvariantCulture, "El nuevo usuario '{0}' se creó exitosamente.", innerUserId)) });
                    }
                    else
                    {
                        globalError = AccountValidation.ErrorCodeToString(status);
                    }
                }
                else
                {
                    var innerUserId = int.Parse(userToCreate.Id, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    if (!this.membershipService.ExistsUser(innerUserId))
                    {
                        status = this.membershipService.CreateUser(innerUserId, userToCreate.Username, userToCreate.Password, userToCreate.Email);
                        if (status == MembershipCreateStatus.Success)
                        {
                            this.AddRoleAndProfile(userToCreate);

                            return this.RedirectToAction("CreateUser", new { msg = Server.UrlEncode(string.Format(CultureInfo.InvariantCulture, "El nuevo usuario '{0}' se creó exitosamente.", innerUserId)) });
                        }
                        else
                        {
                            globalError = AccountValidation.ErrorCodeToString(status);
                        }
                    }
                    else
                    {
                        globalError = "El Identificador de usuario seleccionado ya esta siendo usado por otro usuario.";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(globalError))
            {
                this.ModelState.AddModelError("GlobalError", globalError);
            }

            if ((this.ModelState[""] != null) && (this.ModelState[""].Errors.Count > 0))
            {
                this.ModelState["Password"].Errors.Add(this.ModelState[""].Errors[0].ErrorMessage);
                this.ModelState["ConfirmPassword"].Errors.Add(this.ModelState[""].Errors[0].ErrorMessage);
            }

            return this.View(userToCreate);
        }
        
        [Authorize(Roles = "ITManager")]
        public ActionResult FileLogs()
        {
            return this.View(new FileFilterViewModel { FileType = 5, State = 1 });
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

        [Authorize(Roles = "ITManager")]
        public ActionResult FileLog(int fileId)
        {
            var file = this.metricsRepository.RetrieveProcessedFilesById(fileId);
            IEnumerable<string> logs = new List<string>();

            if ((file != null) && !string.IsNullOrWhiteSpace(file.Log))
            {
                logs = file.Log.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);                
            }

            logs = logs.Select(l => l.Trim().Trim('\n')).Distinct();

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = logs.Select(l => new { Value = l }) };
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

        private void AddRoleAndProfile(UserViewModel user)
        {
            this.membershipService.AddUserToRol(user.Username, (SelfManagementRoles)user.Role);

            decimal? grossSalary = null;
            decimal grossSalaryValue;
            var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            format.CurrencySymbol = "$";
                        
            if (decimal.TryParse(user.GrossSalary, NumberStyle, format, out grossSalaryValue))
            {
                grossSalary = grossSalaryValue;
            }
 
            this.membershipService.CreateProfile(user.Username, user.Dni, user.Names, user.LastName, grossSalary, user.Status == 0 ? "PTE" : "FTE", user.Status == 0 ? "activo" : "inactivo", DateTime.Now);
        }
    }
}
