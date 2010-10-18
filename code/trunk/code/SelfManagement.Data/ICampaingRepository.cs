namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface ICampaingRepository
    {
        IList<Customer> RetrieveCustomersByName(string customerName);

        IList<Metric> RetrieveAvailableMetrics();

        IList<Supervisor> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null);

        Campaing RetrieveUserCurrentCampaing(int innerUserId);
        
        Campaing RetrieveCampaingById(int campaingId);

        IList<Campaing> RetrieveCampaingsByUserId(int innerUserId);

        IList<CampaingMetricLevel> RetrieveCampaingMetricLevels(int campaingId);

        IList<Supervisor> RetrieveCampaingSupervisors(int campaingId);

        IList<Agent> RetrieveCampaingAgents(int campaingId);

        IList<Agent> RetrieveAgentsBySupervisorId(int supervisorId);

        int RetrieveOrCreateCustomerIdByName(string customerName);

        int CreateCampaing(Campaing campaing);

        void SaveCampaingMetricLevels(IEnumerable<CampaingMetricLevel> campaingMetricLevels);

        void SaveCampaingSupervisors(IEnumerable<CampaingUser> campaingSupervisors);
    }
}
