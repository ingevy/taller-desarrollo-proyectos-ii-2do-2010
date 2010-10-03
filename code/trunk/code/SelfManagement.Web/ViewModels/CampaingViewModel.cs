namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Globalization;

    public class CampaingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage="El nombre de la campaña es requerido.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El cliente es requerido.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "El tipo de campaña es requerido.")]
        [Range(0, 1, ErrorMessage = "El tipo de campaña es de entrada o salida.")]
        public int CampaingType { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateCampaingSupervisors")]
        public IList<SupervisorViewModel> CampaingSupervisors { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        [CustomValidation(typeof(CampaingViewModel), "ValidateDate")]
        public string BeginDate { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateDate")]
        public string EndDate { get; set; }

        public string Description { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateCampaingMetrics")]
        public IList<CampaingMetricLevelViewModel> CampaingMetricLevels { get; set; }

        public bool AreDatesValid()
        {
            if (string.IsNullOrWhiteSpace(this.BeginDate))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.EndDate))
            {
                return true;
            }

            DateTime beginDate;
            DateTime endDate;

            if (DateTime.TryParseExact(this.EndDate, "dd/MM/yyyy", CultureInfo.CurrentUICulture, DateTimeStyles.None, out endDate) &&
                DateTime.TryParseExact(this.BeginDate, "dd/MM/yyyy", CultureInfo.CurrentUICulture, DateTimeStyles.None, out beginDate))
            {
                return endDate > beginDate;
            }

            return false;
        }

        public static ValidationResult ValidateDate(string newDate, ValidationContext pValidationContext)
        {
            if (!string.IsNullOrWhiteSpace(newDate))
            {
                DateTime date;
                if (!DateTime.TryParseExact(newDate, "dd/MM/yyyy", CultureInfo.CurrentUICulture, DateTimeStyles.None, out date))
                {
                    return new ValidationResult("Formato de fecha invalido", new List<string> { "BeginDate" });
                }
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateCampaingMetrics(IList<CampaingMetricLevelViewModel> campaingMetrics, ValidationContext pValidationContext)
        {
            if ((campaingMetrics == null) || (campaingMetrics.Where(cm => cm.Selected).Count() != 3))
            {
                return new ValidationResult("Se requiere definir tres metricas para cada Campaña", new List<string> { "CampaingMetricLevels" });
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateCampaingSupervisors(IList<SupervisorViewModel> cmpaingSupervisors, ValidationContext pValidationContext)
        {
            if ((cmpaingSupervisors == null) || (cmpaingSupervisors.Where(cs => cs.Selected).Count() == 0))
            {
                return new ValidationResult("Se requiere al menos un Supervisor para cada Campaña", new List<string> { "CampaingSupervisors" });
            }

            return ValidationResult.Success;
        }
    }
}