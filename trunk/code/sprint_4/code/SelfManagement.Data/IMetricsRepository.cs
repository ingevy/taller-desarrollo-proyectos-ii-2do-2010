namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface IMetricsRepository
    {
        ProcessedFile RetrieveProcessedFileByPath(string filePath);

        IList<Metric> RetrieveAvailableMetrics();

        int RetrieveUserActualCampaingId(int innerUserId);

        int RetrieveUserCampaingId(int innerUserId, DateTime date);

        int RetrieveAgentSupervisorId(int innerUserId);

        MonthlySchedule RetrieveAgentMonthlySchedule(int innerUserId, short year, byte month);

        bool IsHolidayDate(DateTime date);

        void SaveOrUpdateMonthlySchedule(MonthlySchedule schedule);

        double GetUserMetricValue(int innerUserId, DateTime date, int metricId, int campaingId);

        double GetCampaingMetricValue(int campaingId, DateTime date, int metricId);

        void CreateAgentMetric(UserMetric userMetric);

        void CreateSupervisorAgent(SupervisorAgent supervisorAgent);

        void CreateOrUpdateSupervisorMetric(int metricId, DateTime date);

        void CreateOrUpdateCampaingMetric(int metricId, DateTime date);

        int CreateProcessedFile(ProcessedFile file);

        void LogInProcessedFile(string filePath, string logMessage);

        void ChangeAgentSupervisor(int agentId, int newSupervisorId);

        void ChangeAgentSupervisorAndCampaing(int agentId, int newSupervisorId, int newCampaingId);
    }
}
