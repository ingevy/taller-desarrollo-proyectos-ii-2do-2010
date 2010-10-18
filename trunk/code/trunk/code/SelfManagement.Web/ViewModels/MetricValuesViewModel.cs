namespace CallCenter.SelfManagement.Web.ViewModels
{
    public class MetricValuesViewModel
    {
        public int CampaingId { get; set; }

        public int MetricId { get; set; }

        public string Format { get; set; }

        public string MetricName { get; set; }

        public double OptimalValue { get; set; }

        public double ObjectiveValue { get; set; }

        public double MinimumValue { get; set; }

        public double CurrentValue { get; set; }

        public double ProjectedValue { get; set; }
    }
}