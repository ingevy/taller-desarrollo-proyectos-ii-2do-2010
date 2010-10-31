namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface IMetricsRepository
    {
        ProcessedFile RetrieveProcessedFileByPath(string filePath);

        IList<Metric> RetrieveAvailableMetrics();

        int RetrieveUserActualCampaingId(int innerUserId);

        int RetrieveAgentSupervisorId(int innerUserId);

        double GetUserMetricValue(int innerUserId, DateTime date, int metricId, int campaingId);

        void CreateAgentMetric(UserMetric userMetric);

        void CreateSupervisorAgent(SupervisorAgent supervisorAgent);

        void CreateOrUpdateSupervisorMetric(int innerUserId, int campaingId, int metricId, DateTime date, double value);

        void CreateOrUpdateCampaingMetric(int campaingId, int metricId, DateTime date, double value);

        int CreateProcessedFile(ProcessedFile file);
    }
}
