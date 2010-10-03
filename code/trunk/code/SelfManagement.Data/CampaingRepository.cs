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

        public void SaveCampaingMetrics(IEnumerable<CampaingMetricLevel> campaingMetricLevels)
        {
            using (var ctx = new SelfManagementEntities())
            {
                foreach (var campaingMetricLevel in campaingMetricLevels)
                {
                    ctx.CampaingMetricLevels.AddObject(campaingMetricLevel);
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
