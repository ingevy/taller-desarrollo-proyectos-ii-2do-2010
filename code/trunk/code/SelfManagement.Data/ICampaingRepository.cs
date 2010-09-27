namespace CallCenter.SelfManagement.Data
{
    using System.Collections.Generic;

    public interface ICampaingRepository
    {
        IList<Metric> RetrieveAvailableMetrics();

        IList<Customer> SearchCustomer(string text);
    }
}
