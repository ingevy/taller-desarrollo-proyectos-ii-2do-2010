namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface ICampaingRepository
    {
        bool ExistsAgent(int agentId);

        bool ExistsSupervisor(int supervisorId);

        bool ExistsCampaing(int campaingId);

        IList<Customer> RetrieveCustomersByName(string customerName);

        IList<Metric> RetrieveAvailableMetrics();

        IList<Supervisor> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null);

        IList<string> RetrieveAvailableMonthsByCampaing(int campaingId);

        Campaing RetrieveCurrentCampaingByUserId(int innerUserId);
        
        Campaing RetrieveCampaingById(int campaingId);

        IList<Campaing> RetrieveAllCampaings(int pageSize, int pageNumber);

        int CountAllCampaings();

        IList<Campaing> RetrieveCampaingsByUserIdAndDate(int innerUserId, DateTime date);

        IList<Campaing> RetrieveCampaingsByUserId(int innerUserId);

        IList<CampaingMetricLevel> RetrieveCampaingMetricLevels(int campaingId);

        IList<Supervisor> RetrieveCampaingSupervisors(int campaingId);

        int CountCampaingSupervisors(int campaingId);

        IList<Agent> RetrieveCampaingAgents(int campaingId);

        IList<Agent> RetrieveCampaingAgents(int campaingId, int pageSize, int pageNumber);

        int CountCampaingAgents(int campaingId);

        IList<Agent> RetrieveAgentsBySupervisorId(int supervisorId);

        int RetrieveOrCreateCustomerIdByName(string customerName);

        void AddAgent(int campaingId, Agent agent);

        int CreateCampaing(Campaing campaing);

        void SaveCampaingMetricLevels(IEnumerable<CampaingMetricLevel> campaingMetricLevels);

        void SaveCampaingSupervisors(IEnumerable<CampaingUser> campaingSupervisors);

        void EndCampaing(int campaingId);
    }
}
