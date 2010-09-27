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

        public IList<Customer> SearchCustomer(string text)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Customers
                    .FullTextSearch(text)
                    .ToList();
            }
        }

        public IList<UserProfile> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null)
        {
            using (var ctx = new SelfManagementEntities())
            {
                IQueryable<UserProfile> query;

                if (endDate.HasValue)
                {
                    var finalDate = endDate.Value;

                    query = from up in ctx.UserProfiles
                            where (up.Role == "Supervisor") && ctx.CampaingUsers
                                                                    .Where(cu => cu.InnerUserId == up.InnerUserId)
                                                                    .All(cu => ((beginDate < cu.BeginDate) || (beginDate > cu.EndDate)) && ((finalDate < cu.BeginDate) || (finalDate > cu.EndDate)))
                            select up;
                }
                else
                {
                    query = from up in ctx.UserProfiles
                            where (up.Role == "Supervisor") && ctx.CampaingUsers
                                                                    .Where(cu => cu.InnerUserId == up.InnerUserId)
                                                                    .All(cu => beginDate > cu.EndDate)
                            select up;
                }

                return query.ToList();
            }
        }

        public Customer GetOrCreateCustomerByName(string customerName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var customer = ctx.Customers
                    .FullTextSearch(customerName, true)
                    .FirstOrDefault();

                if (customer == null)
                {
                    customer = new Customer { Name = customerName };
                    ctx.AddToCustomers(customer);
                    ctx.SaveChanges();

                    customer = ctx.Customers
                        .FullTextSearch(customerName, true)
                        .FirstOrDefault();
                }

                return customer;
            }
        }

        public int SaveCampaing(Campaing campaing)
        {
            using (var ctx = new SelfManagementEntities())
            {
                ctx.AddToCampaings(campaing);
                ctx.SaveChanges();

                var campaingId = ctx.Campaings
                    .Where(c => (c.Name == campaing.Name) && (c.SupervisorId == campaing.SupervisorId) && (c.CampaingType == campaing.CampaingType))
                    .Select(c => c.Id)
                    .FirstOrDefault();

                var campaingUser = new CampaingUser
                {
                    CampaingId = campaingId,
                    InnerUserId = campaing.SupervisorId,
                    BeginDate = campaing.BeginDate,
                    EndDate = campaing.EndDate.HasValue ? campaing.EndDate.Value : DateTime.MaxValue 
                };

                ctx.AddToCampaingUsers(campaingUser);
                ctx.SaveChanges();

                return campaingId;
            }
        }

        public void SaveCampaingMetrics(IEnumerable<CampaingMetricLevel> campaingMetricLevels)
        {
            using (var ctx = new SelfManagementEntities())
            {
                foreach (var campaingMetricLevel in campaingMetricLevels)
                {
                    ctx.AddToCampaingMetricLevels(campaingMetricLevel);
                }

                ctx.SaveChanges();
            }
        }
    }
}
