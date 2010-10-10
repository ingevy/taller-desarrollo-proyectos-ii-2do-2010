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
            var entity = new Campaing
            {
                Name = model.Name,
                Description = model.Description,
                BeginDate = DateTime.ParseExact(model.BeginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                EndDate = string.IsNullOrWhiteSpace(model.EndDate) ? (DateTime?)null : DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                CampaingType = model.CampaingType,
                OptimalHourlyValue = decimal.Parse(model.OptimalHourlyValue, NumberStyle, CultureInfo.InvariantCulture),
                ObjectiveHourlyValue = decimal.Parse(model.ObjectiveHourlyValue, NumberStyle, CultureInfo.InvariantCulture),
                MinimumHourlyValue = decimal.Parse(model.MinimumHourlyValue, NumberStyle, CultureInfo.InvariantCulture),
                CustomerId = repository.RetrieveOrCreateCustomerIdByName(model.CustomerName),
            };

            return entity;
        }

        public static CampaingMetricLevel ToEntity(this CampaingMetricLevelViewModel model, int campaingId)
        {
            var entity = new CampaingMetricLevel
            {
                CampaingId = campaingId,
                MetricId = model.Id,
                OptimalLevel = double.Parse(model.OptimalLevel, NumberStyle, CultureInfo.InvariantCulture),
                ObjectiveLevel = double.Parse(model.ObjectiveLevel, NumberStyle, CultureInfo.InvariantCulture),
                MinimumLevel = double.Parse(model.MinimumLevel, NumberStyle, CultureInfo.InvariantCulture),
                Enabled = true
            };

            return entity;
        }

        public static CampaingUser ToEntity(this SupervisorViewModel model, int campaingId, string beginDate, string endDate)
        {
            var entity = new CampaingUser
            {
                InnerUserId = model.Id,
                CampaingId = campaingId,
                BeginDate = DateTime.ParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                EndDate = string.IsNullOrWhiteSpace(endDate) ? DateTime.MaxValue : DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)
            };

            return entity;
        }

        public static SupervisorViewModel ToViewModel(this Supervisor entity)
        {
            var model = new SupervisorViewModel
            {
                Id = entity.InnerUserId,
                DisplayName = GetDisplayName(entity),
                Selected = false
            };

            return model;
        }

        public static CampaingMetricLevelViewModel ToViewModel(this Metric entity)
        {
            var model = new CampaingMetricLevelViewModel
            {
                Id = entity.Id,
                Name = entity.MetricName,
                Description = entity.ShortDescription,
                FormatType = entity.Format,
                IsHighestToLowest = entity.IsHighestToLowest,
                Selected = false,
            };

            return model;
        }

        private static string GetDisplayName(Supervisor supervisor)
        {
            if (!string.IsNullOrEmpty(supervisor.Name) && !string.IsNullOrEmpty(supervisor.LastName))
            {
                return string.Format(CultureInfo.CurrentUICulture, "{0} {1} ({2})", supervisor.Name, supervisor.LastName, supervisor.InnerUserId);
            }

            return string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", supervisor.UserName, supervisor.InnerUserId);
        } 
    }
}