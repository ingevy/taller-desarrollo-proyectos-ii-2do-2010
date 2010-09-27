namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    public class CampaingMetricLevelViewModel
    {
        [Required]
        public int MetricId { get; set; }

        public int FormatType { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage="El nivel optimo de la métrica es requerido.")]
        public string OptimalLevel { get; set; }

        [Required(ErrorMessage = "El nivel objetivo de la métrica es requerido.")]
        public string ObjectiveLevel { get; set; }

        [Required(ErrorMessage = "El nivel minimo de la métrica es requerido.")]
        public string MinimumLevel { get; set; }

        [Required]
        public string MetricLevelStatus { get; set; }
    }
}