namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface ICampaingRepository
    {
        IList<Customer> RetrieveCustomersByName(string customerName);

        IList<Metric> RetrieveAvailableMetrics();

        IList<Supervisor> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null);

        Campaing RetrieveCampaingById(int campaingId);

        int RetrieveOrCreateCustomerIdByName(string customerName);

        int CreateCampaing(Campaing campaing);

        void SaveCampaingMetrics(IEnumerable<CampaingMetricLevel> campaingMetricLevels);

        void SaveCampaingSupervisors(IEnumerable<CampaingUser> campaingSupervisors);
    }
}
