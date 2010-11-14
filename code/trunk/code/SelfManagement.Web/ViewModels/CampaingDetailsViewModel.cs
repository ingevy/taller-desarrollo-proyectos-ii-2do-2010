namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CampaingDetailsViewModel
    {
        public int CampaingId { get; set; }

        public string DisplayName { get; set; }

        [DisplayName("Supervisores")]
        public int SupervisorsCount { get; set; }

        [DisplayName("Agentes")]
        public int AgentsCount { get; set; }

        [DisplayName("Cliente")]
        public string Customer { get; set; }

        [DisplayName("Tipo")]
        public string CampaingType { get; set; }

        [DisplayName("Fecha de Inicio")]
        public string BeginDate { get; set; }

        [DisplayName("Fecha de Fin")]
        public string EndDate { get; set; }       

        [DisplayName("Optimo $/h")]
        public string OptimalHourlyValue { get; set; }

        [DisplayName("Objetivo $/h")]
        public string ObjectiveHourlyValue { get; set; }

        [DisplayName("Minimo $/h")]
        public string MinimumHourlyValue { get; set; }
        
        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Nivel Actual")]
        public MetricLevel CurrentMetricLevel { get; set; }

        [DisplayName("Nivel Proyectado")]
        public MetricLevel ProjectedMetricLevel { get; set; }

        [DisplayName("Mes")]
        public int CurrentMetricMonthIndex { get; set; }

        public IList<MetricValuesViewModel> MetricValues { get; set; }

        public IList<string> AvailableMetricMonths { get; set; }

        public bool ShowEndCampaing { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }
    }
}