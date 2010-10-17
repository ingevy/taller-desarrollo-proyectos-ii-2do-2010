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

        void CreateAgentMetric(UserMetric userMetric);

        void CreateOrUpdateSupervisorMetric(int innerUserId, int campaingId, int metricId, DateTime date, double value);

        void CreateOrUpdateCampaingMetric(int campaingId, int metricId, DateTime date, double value);

        int CreateProcessedFile(ProcessedFile file);
    }
}
