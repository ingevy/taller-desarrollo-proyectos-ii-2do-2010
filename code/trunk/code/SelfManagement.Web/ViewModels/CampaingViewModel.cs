namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Se requiere un supervisor.")]
        public int SupervisorId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        [CustomValidation(typeof(CampaingViewModel), "ValidateDate")]
        public string BeginDate { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateDate")]
        public string EndDate { get; set; }

        public string Description { get; set; }

        public IList<SupervisorViewModel> Supervisors { get; set; }

        public IList<MetricViewModel> Metrics { get; set; }

        [Required(ErrorMessage = "Se requiere definir tres metricas para la campaña.")]
        public IList<CampaingMetricLevelViewModel> CampaingMetrics { get; set; }

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
    }
}