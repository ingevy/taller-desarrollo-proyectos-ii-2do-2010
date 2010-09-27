namespace CallCenter.SelfManagement.Web.Helpers
{
    using System;
    using System.Globalization;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.ViewModels;

    public static class EntityTranslator
    {
        public static Campaing ToEntity(this CampaingViewModel model, ICampaingRepository repository)
        {
            var data = new Campaing
            {
                Name = model.Name,
                Description = model.Description,
                BeginDate = DateTime.ParseExact(model.BeginDate, "dd/MM/yyyy", CultureInfo.CurrentUICulture, DateTimeStyles.None),
                EndDate = string.IsNullOrWhiteSpace(model.EndDate) ? (DateTime?)null : DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", CultureInfo.CurrentUICulture, DateTimeStyles.None),
                CampaingType = model.CampaingType,
                CustomerId = repository.GetOrCreateCustomerByName(model.CustomerName).Id,
                SupervisorId = model.SupervisorId
            };

            return data;
        }

        public static CampaingMetricLevel ToEntity(this CampaingMetricLevelViewModel model, int campaingId)
        {
            var data = new CampaingMetricLevel
            {
                CampaingId = campaingId,
                MetricId = model.MetricId,
                OptimalLevel = double.Parse(model.OptimalLevel),
                ObjectiveLevel = double.Parse(model.ObjectiveLevel),
                MinimumLevel = double.Parse(model.MinimumLevel)
            };

            return data;
        }
    }
}