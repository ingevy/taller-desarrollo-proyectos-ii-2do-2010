namespace CallCenter.SelfManagement.Data
{
    using System.Collections.Generic;
    using System.Linq;

    public class CampaingRepository : ICampaingRepository
    {
        public IList<Metric> RetrieveAvailableMetrics()
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from metric in ctx.Metrics
                            select metric;

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
    }
}
