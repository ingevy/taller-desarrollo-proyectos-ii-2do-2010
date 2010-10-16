namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface IMetricsRepository
    {
        ProcessedFile RetrieveProcessedFileByPath(string filePath);

        IList<Metric> RetrieveAvailableMetrics();

        Campaing RetrieveAgentActualCampaing(int innerUserId);

        void CreateUserMetric(UserMetric userMetric);

        void CreateOrUpdateCampaingMetric(int campaingId, int metricId, DateTime date, double value);

        int CreateProcessedFile(ProcessedFile file);
    }
}
