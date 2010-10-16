namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MetricsRepository : IMetricsRepository
    {

        public ProcessedFile RetrieveProcessedFileByPath(string filePath)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.ProcessedFiles
                    .Where(pf => pf.FileSystemPath == filePath )
                    .FirstOrDefault();
            }
        }

        public IList<Metric> RetrieveAvailableMetrics()
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from m in ctx.Metrics
                            select m;

                return query.ToList();
            }
        }

        public Campaing RetrieveAgentActualCampaing(int innerUserId)
        {
            throw new NotImplementedException();
        }

        public int SaveUserMetric(UserMetric userMetric)
        {
            throw new NotImplementedException();
        }

        public int SaveOrUpdateCampaingMetric(int campaingId, int metricId, DateTime date, double value)
        {
            throw new NotImplementedException();
        }

        public int SaveProcessedFile(ProcessedFile file)
        {
            throw new NotImplementedException();
        }
    }
}
