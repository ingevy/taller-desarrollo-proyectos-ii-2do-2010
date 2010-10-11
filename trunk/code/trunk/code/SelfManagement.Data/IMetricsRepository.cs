namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;

    public interface IMetricsRepository
    {
        ProcessedFile RetrieveProcessedFileByPath(string filePath);

        IList<Metric> RetrieveAvailableMetrics();

        Campaing RetrieveAgentActualCampaing(int innerUserId);

        int RetrieveInnerUserIdByEmployeeId(int employeeId);

        int SaveUserMetric(UserMetric userMetric);

        int SaveOrUpdateCampaingMetric(int campaingId, int metricId, DateTime date, double value);

        int SaveProcessedFile(ProcessedFile file);

    }
}
