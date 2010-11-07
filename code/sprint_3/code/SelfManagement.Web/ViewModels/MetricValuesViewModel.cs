namespace CallCenter.SelfManagement.Web.ViewModels
{
    public class MetricValuesViewModel
    {
        public int CampaingId { get; set; }

        public int MetricId { get; set; }

        public string Format { get; set; }

        public string MetricName { get; set; }

        public string OptimalValue { get; set; }

        public string ObjectiveValue { get; set; }

        public string MinimumValue { get; set; }

        public string CurrentValue { get; set; }

        public string ProjectedValue { get; set; }

        public string OptimalHourlyValue { get; set; }

        public string ObjectiveHourlyValue { get; set; }

        public string MinimumHourlyValue { get; set; } 
    }
}