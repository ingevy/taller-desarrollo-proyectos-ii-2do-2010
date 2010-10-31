namespace CallCenter.SelfManagement.Web.ViewModels
{
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
        public int CurrentMonth
        {
            get { return this.AvailableMonths.Count - 1; }
        }

        public SalaryViewModel Salary { get; set; }

        [DisplayName("Optimo $/h")]
        public string OptimalHourlyValue { get; set; }

        [DisplayName("Objetivo $/h")]
        public string ObjectiveHourlyValue { get; set; }

        [DisplayName("Minimo $/h")]
        public string MinimumHourlyValue { get; set; }

        public IList<MetricValuesViewModel> CurrentCampaingMetricValues { get; set; }

        public IList<string> AvailableMonths { get; set; }

        public IList<UserCampaingInfo> AgentCampaings { get; set; }
    }
}