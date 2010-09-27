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
    }
}
