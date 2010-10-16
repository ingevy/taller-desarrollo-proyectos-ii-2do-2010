namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Globalization;

    public class CampaingViewModel
    {
        private const NumberStyles NumberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number;

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la campaña es requerido.")]
        [StringLength(100, ErrorMessage="El nombre de la campaña debe tener menos de 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El cliente es requerido.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "El tipo de campaña es requerido.")]
        [Range(0, 1, ErrorMessage = "El tipo de campaña es de entrada o salida.")]
        public int CampaingType { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        [CustomValidation(typeof(CampaingViewModel), "ValidateDate")]
        public string BeginDate { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateDate")]
        public string EndDate { get; set; }

        [StringLength(750, ErrorMessage = "La descripcion de la campaña debe tener menos de 750 caracteres.")]
        public string Description { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateNumber")]
        [Required(ErrorMessage = "El valor de la hora del nivel optimo es requerido.")]
        public string OptimalHourlyValue { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateNumber")]
        [Required(ErrorMessage = "El valor de la hora del nivel objetivo es requerido.")]
        public string ObjectiveHourlyValue { get; set; }

        [CustomValidation(typeof(CampaingViewModel), "ValidateNumber")]
        [Required(ErrorMessage = "El valor de la hora del nivel mínimo es requerido.")]
        public string MinimumHourlyValue { get; set; } 

        [Range(3, 3, ErrorMessage = "Se requiere definir tres metricas para cada Campaña.")]
        public int CampaingMetricLevelsCount
        {
            get { return this.CampaingMetricLevels.Where(cml => cml.Selected).Count(); }
        }

        [Range(1, int.MaxValue, ErrorMessage = "Se requiere al menos un Supervisor para cada Campaña.")]
        public int CampaingSupervisorsCount
        {
            get
            {
                if (this.CampaingSupervisors != null)
                {
                    return this.CampaingSupervisors.Where(s => s.Selected).Count();
                }

                return 0;
            }
        }

        [Range(1, 1, ErrorMessage = "El valor de la hora para el nivel optimo debe ser el mayor.")]
        public int OptimalHourlyValueValid
        {
            get
            {
                decimal optimalHourlyValue;
                decimal objectiveHourlyValue;
                decimal minimumHourlyValue;

                if (decimal.TryParse(this.OptimalHourlyValue, NumberStyle, CultureInfo.InvariantCulture, out optimalHourlyValue) &&
                    decimal.TryParse(this.ObjectiveHourlyValue, NumberStyle, CultureInfo.InvariantCulture, out objectiveHourlyValue) &&
                    decimal.TryParse(this.MinimumHourlyValue, NumberStyle, CultureInfo.InvariantCulture, out minimumHourlyValue))
                {
                    return (optimalHourlyValue > objectiveHourlyValue) && (optimalHourlyValue > minimumHourlyValue)
                           ? 1
                           : 0;
                }

                return 1;
            }
        }

        [Range(1, 1, ErrorMessage = "El valor de la hora para el nivel objetivo debe ser el mayor al minimo.")]
        public int ObjectiveHourlyValueValid
        {
            get
            {
                decimal objectiveHourlyValue;
                decimal minimumHourlyValue;

                if (decimal.TryParse(this.ObjectiveHourlyValue, NumberStyle, CultureInfo.InvariantCulture, out objectiveHourlyValue) &&
                    decimal.TryParse(this.MinimumHourlyValue, NumberStyle, CultureInfo.InvariantCulture, out minimumHourlyValue))
                {
                    return objectiveHourlyValue > minimumHourlyValue
                           ? 1
                           : 0;
                }

                return 1;
            }
        }

        public IList<CampaingMetricLevelViewModel> CampaingMetricLevels { get; set; }

        public IList<SupervisorViewModel> CampaingSupervisors { get; set; }

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

        public static ValidationResult ValidateDate(string newDate, ValidationContext validationContext)
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

        public static ValidationResult ValidateNumber(string number, ValidationContext validationContext)
        {
            double result = 0;

            if (!double.TryParse(number, NumberStyle, CultureInfo.InvariantCulture, out result))
            {
                return new ValidationResult("Formato inválido.");
            }

            if (result < 0)
            {
                return new ValidationResult("El valor de hora no puede ser negativo.");
            }

            return ValidationResult.Success;
        }
    }
}