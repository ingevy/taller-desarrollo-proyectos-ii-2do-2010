namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    public class CampaingMetricLevelViewModel
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int FormatType { get; set; }

        public bool Selected { get; set; }

        public bool IsHighestToLowest { get; set; }

        [Required(ErrorMessage = "El nivel optimo de la métrica es requerido.")]
        [CustomValidation(typeof(CampaingMetricLevelViewModel), "ValidateLevel")]
        public string OptimalLevel { get; set; }

        [Required(ErrorMessage = "El nivel objetivo de la métrica es requerido.")]
        [CustomValidation(typeof(CampaingMetricLevelViewModel), "ValidateLevel")]
        public string ObjectiveLevel { get; set; }

        [Required(ErrorMessage = "El nivel minimo de la métrica es requerido.")]
        [CustomValidation(typeof(CampaingMetricLevelViewModel), "ValidateLevel")]
        public string MinimumLevel { get; set; }

        public static ValidationResult ValidateLevel(string level, ValidationContext validationContext)
        {
            var numberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number;
            double result = 0;

            if (!double.TryParse(level, numberStyle, CultureInfo.InvariantCulture, out result))
            {
                return new ValidationResult("Formato inválido.");
            }

            return ValidationResult.Success;
        }
    }
}