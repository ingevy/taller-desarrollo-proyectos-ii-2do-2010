namespace CallCenter.SelfManagement.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class AgentDetailsViewModel
    {
        public int AgentId { get; set; }

        public string DisplayName { get; set; }

        [DisplayName("Supervisor Actual")]
        public string CurrentSupervisor { get; set; }

        [DisplayName("Campañas")]
        public int CurrentCampaingId { get; set; }

        [DisplayName("Mes")]
        public int CurrentSalaryMonthIndex
        {
            get { return this.AvailableSalaryMonths.Count - 1; }
        }

        [DisplayName("Mes")]
        public int CurrentMetricMonthIndex
        {
            get
            {
                var index = this.AvailableMetricMonths.IndexOf(DateTime.Now.ToString("yyyy-MM"));

                return (index > -1) ? index : this.AvailableMetricMonths.Count - 1;
            }
        }

        public SalaryViewModel Salary { get; set; }

        [DisplayName("Optimo $/h")]
        public string OptimalHourlyValue { get; set; }

        [DisplayName("Objetivo $/h")]
        public string ObjectiveHourlyValue { get; set; }

        [DisplayName("Minimo $/h")]
        public string MinimumHourlyValue { get; set; }

        [DisplayName("Nivel Actual")]
        public MetricLevel CurrentMetricLevel { get; set; }

        [DisplayName("Nivel Proyectado")]
        public MetricLevel ProjectedMetricLevel { get; set; }

        public IList<MetricValuesViewModel> CurrentCampaingMetricValues { get; set; }

        public IList<string> AvailableSalaryMonths { get; set; }

        public IList<string> AvailableMetricMonths { get; set; }

        public IList<UserCampaingInfo> AgentCampaings { get; set; }
    }
}