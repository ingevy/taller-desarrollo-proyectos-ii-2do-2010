namespace CallCenter.SelfManagement.Web.Helpers
{
    using System;
    using System.Globalization;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.ViewModels;

    public static class EntityTranslator
    {
        private const NumberStyles NumberStyle = NumberStyles.Float | NumberStyles.Integer | NumberStyles.Number;

        public static Campaing ToEntity(this CampaingViewModel model, ICampaingRepository repository)
        {
            var data = new Campaing
            {
                Name = model.Name,
                Description = model.Description,
                BeginDate = DateTime.ParseExact(model.BeginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                EndDate = string.IsNullOrWhiteSpace(model.EndDate) ? (DateTime?)null : DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                CampaingType = model.CampaingType,
                CustomerId = repository.RetrieveOrCreateCustomerIdByName(model.CustomerName),
            };

            return data;
        }

        public static CampaingMetricLevel ToEntity(this CampaingMetricLevelViewModel model, int campaingId)
        {
            var data = new CampaingMetricLevel
            {
                CampaingId = campaingId,
                MetricId = model.Id,
                OptimalLevel = double.Parse(model.OptimalLevel, NumberStyle, CultureInfo.InvariantCulture),
                ObjectiveLevel = double.Parse(model.ObjectiveLevel, NumberStyle, CultureInfo.InvariantCulture),
                MinimumLevel = double.Parse(model.MinimumLevel, NumberStyle, CultureInfo.InvariantCulture),
                Enabled = true
            };

            return data;
        }

        public static CampaingUser ToEntity(this SupervisorViewModel model, int campaingId, string beginDate, string endDate)
        {
            var data = new CampaingUser
            {
                InnerUserId = model.Id,
                CampaingId = campaingId,
                BeginDate = DateTime.ParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                EndDate = string.IsNullOrWhiteSpace(endDate) ? DateTime.MaxValue : DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
            };

            return data;
        }
    }
}