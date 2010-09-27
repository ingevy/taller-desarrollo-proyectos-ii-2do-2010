namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface ICampaingRepository
    {
        IList<Metric> RetrieveAvailableMetrics();

        IList<Customer> SearchCustomer(string text);

        IList<UserProfile> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null);
    }
}
