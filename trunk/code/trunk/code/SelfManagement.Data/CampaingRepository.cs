namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CampaingRepository : ICampaingRepository
    {
        public IList<Metric> RetrieveAvailableMetrics()
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from m in ctx.Metrics
                            select m;

                return query.ToList();
            }
        }

        public IList<Customer> RetrieveCustomersByName(string customerName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Customers
                    .FullTextSearch(customerName)
                    .ToList();
            }
        }

        public IList<Supervisor> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null)
        {
            using (var ctx = new SelfManagementEntities())
            {
                IQueryable<Supervisor> query;

                if (endDate.HasValue)
                {
                    var finalDate = endDate.Value;

                    query = from s in ctx.Supervisors
                            where ctx.CampaingUsers
                                    .Where(cu => cu.InnerUserId == s.InnerUserId)
                                    .All(cu => ((beginDate < cu.BeginDate) || (beginDate > cu.EndDate)) && ((finalDate < cu.BeginDate) || (finalDate > cu.EndDate)))
                            select s;
                }
                else
                {
                    query = from s in ctx.Supervisors
                            where ctx.CampaingUsers
                                    .Where(cu => cu.InnerUserId == s.InnerUserId)
                                    .All(cu => beginDate > cu.EndDate)
                            select s;
                }

                return query.ToList();
            }
        }

        public Campaing RetrieveCampaingById(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Campaings
                    .Where(c => c.Id == campaingId)
                    .FirstOrDefault();
            }
        }

        public IList<CampaingMetricLevel> RetrieveCampaingMetricLevels(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.CampaingMetricLevels
                    .Where(cml => cml.CampaingId == campaingId)
                    .ToList();
            }
        }

        public IList<Supervisor> RetrieveCampaingSupervisors(int campaingId)
        {
            throw new NotImplementedException();
        }

        public IList<Agent> RetrieveAgentsBySupervisorId(int supervisorId)
        {
            throw new NotImplementedException();
        }

        public int RetrieveOrCreateCustomerIdByName(string customerName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var customer = ctx.Customers
                    .Where(c => c.Name == customerName)
                    .FirstOrDefault();

                if (customer == null)
                {
                    customer = new Customer { Name = customerName };
                    ctx.Customers.AddObject(customer);
                    ctx.SaveChanges();
                }

                return customer.Id;
            }
        }

        public int CreateCampaing(Campaing campaing)
        {
            using (var ctx = new SelfManagementEntities())
            {
                ctx.Campaings.AddObject(campaing);
                ctx.SaveChanges();

                return campaing.Id;
            }
        }

        public void SaveCampaingMetricLevels(IEnumerable<CampaingMetricLevel> campaingMetricLevels)
        {
            if (campaingMetricLevels.Where(cml => cml.Enabled).Count() != 3)
            {
                throw new ArgumentException("Por cada campaña se requieren tres métricas obligatoriamente.");
            }

            var campaingId = campaingMetricLevels.FirstOrDefault().CampaingId;
            if (!campaingMetricLevels.All(cml => cml.CampaingId == campaingId))
            {
                throw new ArgumentException("Todas las metricas deben pertenecer a una misma campaña.");
            }

            using (var ctx = new SelfManagementEntities())
            {
                foreach (var campaingMetricLevel in campaingMetricLevels)
                {
                    var original = ctx.CampaingMetricLevels
                        .Where(cml => (cml.CampaingId == campaingMetricLevel.CampaingId) && (cml.MetricId == campaingMetricLevel.MetricId))
                        .FirstOrDefault();

                    if (original != null)
                    {
                        original.OptimalLevel = campaingMetricLevel.OptimalLevel;
                        original.ObjectiveLevel = campaingMetricLevel.ObjectiveLevel;
                        original.MinimumLevel = campaingMetricLevel.MinimumLevel;
                        original.Enabled = campaingMetricLevel.Enabled;
                    }
                    else
                    {
                        ctx.CampaingMetricLevels.AddObject(campaingMetricLevel);
                    }
                }

                var metricIds = campaingMetricLevels.Select(cml => cml.MetricId).ToList();
                var campaingMetricLevelsToDisable = ctx.CampaingMetricLevels
                    .Where(cml => cml.CampaingId == campaingId)
                    .Where(cml => !metricIds.Contains(cml.MetricId));

                foreach (var campaingMetricLevel in campaingMetricLevelsToDisable)
                {
                    campaingMetricLevel.Enabled = false;
                }                    

                ctx.SaveChanges();
            }
        }

        public void SaveCampaingSupervisors(IEnumerable<CampaingUser> campaingSupervisors)
        {
            throw new NotImplementedException();
        }
    }
}
