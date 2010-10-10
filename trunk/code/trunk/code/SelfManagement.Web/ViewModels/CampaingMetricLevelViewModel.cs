namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    public class CampaingMetricLevelViewModel
    {
        private const NumberStyles NumberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number;

        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int FormatType { get; set; }

        public bool Selected { get; set; }

        public bool IsHighestToLowest { get; set; }

        [Required(ErrorMessage = "El nivel optimo de la métrica es requerido.")]
        [CustomValidation(typeof(CampaingMetricLevelViewModel), "ValidateNumber")]
        public string OptimalLevel { get; set; }

        [Required(ErrorMessage = "El nivel objetivo de la métrica es requerido.")]
        [CustomValidation(typeof(CampaingMetricLevelViewModel), "ValidateNumber")]
        public string ObjectiveLevel { get; set; }

        [Required(ErrorMessage = "El nivel minimo de la métrica es requerido.")]
        [CustomValidation(typeof(CampaingMetricLevelViewModel), "ValidateNumber")]
        public string MinimumLevel { get; set; }

        [Range(1, 1, ErrorMessage = "El nivel optimo no es valido para la metrica.")]
        public int OptimalLevelValid
        {
            get
            {
                double optimalLevel;

                if (double.TryParse(this.OptimalLevel, NumberStyle, CultureInfo.InvariantCulture, out optimalLevel))
                {
                    if ((this.FormatType == 0) && (optimalLevel > 1))
                    {
                        return 0;
                    }

                    double objectiveLevel;
                    double minimumLevel;

                    if (double.TryParse(this.ObjectiveLevel, NumberStyle, CultureInfo.InvariantCulture, out objectiveLevel) &&
                        double.TryParse(this.MinimumLevel, NumberStyle, CultureInfo.InvariantCulture, out minimumLevel))
                    {
                        if (!this.IsHighestToLowest)
                        {
                            return (optimalLevel < objectiveLevel) && (optimalLevel < minimumLevel)
                                    ? 1
                                    : 0;
                        }

                        return (optimalLevel > objectiveLevel) && (optimalLevel > minimumLevel)
                                    ? 1
                                    : 0;
                    }
                }                

                return 1;
            }
        }

        [Range(1, 1, ErrorMessage = "El nivel objetivo no es valido para la metrica.")]
        public int ObjectiveLevelValid
        {
            get
            {
                double objectiveLevel;

                if (double.TryParse(this.ObjectiveLevel, NumberStyle, CultureInfo.InvariantCulture, out objectiveLevel))
                {
                    if ((this.FormatType == 0) && (objectiveLevel > 1))
                    {
                        return 0;
                    }

                    double minimumLevel;

                    if (double.TryParse(this.MinimumLevel, NumberStyle, CultureInfo.InvariantCulture, out minimumLevel))
                    {
                        if (!this.IsHighestToLowest)
                        {
                            return objectiveLevel < minimumLevel
                                    ? 1
                                    : 0;
                        }

                        return objectiveLevel > minimumLevel
                                    ? 1
                                    : 0;
                    }
                }

                return 1;
            }
        }

        [Range(1, 1, ErrorMessage = "El nivel mínimo no es valido para la metrica.")]
        public int MinimumLevelValid
        {
            get
            {
                double minimumLevel;

                if (double.TryParse(this.MinimumLevel, NumberStyle, CultureInfo.InvariantCulture, out minimumLevel))
                {
                    return ((this.FormatType == 0) && (minimumLevel > 1))
                           ? 0
                           : 1;
                }

                return 1;
            }
        }

        public static ValidationResult ValidateNumber(string number, ValidationContext validationContext)
        {
            var numberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number;
            double result = 0;

            if (!double.TryParse(number, numberStyle, CultureInfo.InvariantCulture, out result))
            {
                return new ValidationResult("Formato inválido.");
            }

            if (result < 0)
            {
                return new ValidationResult("El valor del nivel no puede ser negativo.");
            }

            return ValidationResult.Success;
        }
    }
}