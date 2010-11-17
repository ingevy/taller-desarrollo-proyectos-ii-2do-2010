namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class SupervisorDetailsViewModel
    {
        public int SupervisorId { get; set; }

        public string DisplayName { get; set; }

        [DisplayName("Agentes:")]
        public string AgentsCount { get; set; }

        [DisplayName("Campañas")]
        public int CurrentCampaingId { get; set; }
        
        [DisplayName("Mes")]
        public int CurrentMetricMonthIndex
        {
            get
            {
                var index = this.AvailableMetricMonths.IndexOf(DateTime.Now.ToString("yyyy-MM"));

                return (index > -1) ? index : this.AvailableMetricMonths.Count - 1;
            }
        }
        
        [DisplayName("Nivel Actual")]
        public MetricLevel CurrentMetricLevel { get; set; }

        [DisplayName("Nivel Proyectado")]
        public MetricLevel ProjectedMetricLevel { get; set; }

        public bool ShouldPaginate { get; set; }

        public bool ShouldIncludeCampaing { get; set; }

        public int CampaingIdForPagination { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public IList<MetricValuesViewModel> CurrentCampaingMetricValues { get; set; }

        public IList<string> AvailableMetricMonths { get; set; }

        public IList<UserCampaingInfo> SupervisorCampaings { get; set; }
    }
}