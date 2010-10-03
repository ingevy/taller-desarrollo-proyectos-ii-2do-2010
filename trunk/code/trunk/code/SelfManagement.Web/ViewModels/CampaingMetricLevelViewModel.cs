namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;

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
        public string OptimalLevel { get; set; }

        [Required(ErrorMessage = "El nivel objetivo de la métrica es requerido.")]
        public string ObjectiveLevel { get; set; }

        [Required(ErrorMessage = "El nivel minimo de la métrica es requerido.")]
        public string MinimumLevel { get; set; }
    }
}