namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CallCenter.SelfManagement.Data.Helpers;

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

        public int RetrieveUserActualCampaingId(int innerUserId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Campaings
                        .Where(c => ctx.CampaingUsers.Any(cu => (cu.CampaingId == c.Id) && (cu.InnerUserId == innerUserId) 
                                                          && (cu.BeginDate <= DateTime.Now) && (cu.EndDate >= DateTime.Now)))
                        .FirstOrDefault().Id;
            }
        }

        public int RetrieveAgentSupervisorId(int innerUserId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.SupervisorAgents
                        .Where(sa => sa.AgentId == innerUserId)
                        .FirstOrDefault().SupervisorId;
            }
        }

        public double GetUserMetricValue(string userName, DateTime date, int metricId, int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var innerUserId = (from u in ctx.aspnet_Users
                                    where u.UserName == userName
                                    select u.InnerUserId).ToList().FirstOrDefault();

                //var campaingId = this.RetrieveUserActualCampaingId(innerUserId);
                var campaing = ctx.Campaings.Where(c => c.Id == campaingId).ToList().FirstOrDefault();
                var metric = ctx.Metrics.Where(m => m.Id == metricId).ToList().FirstOrDefault();

                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lowLimitDate = (campaing.BeginDate < firstDayOfMonth) ? firstDayOfMonth : campaing.BeginDate;

                var userMetrics = (from m in ctx.UserMetrics
                                   where m.InnerUserId == innerUserId && m.Date >= lowLimitDate && m.Date <= date
                                         && m.MetricId == metricId && m.CampaingId == campaing.Id
                                   select m).ToList();
                
                var metricsCalculator = new MetricsCalculator();
                var metricValue = 0.0;

                if (metric.Format == 0)
                {
                    metricValue = metricsCalculator.CalculateAverageMetricValue(userMetrics, date);
                }
                else
                {
                    metricValue = metricsCalculator.CalculateAcumulatedMetricValue(userMetrics, date);
                }

                return metricValue;
            }
        }

        public void CreateAgentMetric(UserMetric userMetric)
        {
            using (var ctx = new SelfManagementEntities())
            {
                ctx.UserMetrics.AddObject(userMetric);
                ctx.SaveChanges();
            }
        }

        public void CreateOrUpdateSupervisorMetric(int innerUserId, int campaingId, int metricId, DateTime date, double value)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var original = ctx.UserMetrics
                                  .Where(um => (um.InnerUserId == innerUserId) && (um.CampaingId == campaingId) && (um.MetricId == metricId) && (um.Date == date))
                                  .FirstOrDefault();

                if (original != null)
                {
                    original.Value += value;
                }
                else
                {
                    ctx.UserMetrics.AddObject(new UserMetric { InnerUserId = innerUserId, CampaingId = campaingId, MetricId = metricId, Date = date, Value = value });
                }

                ctx.SaveChanges();
            }
        }

        public void CreateOrUpdateCampaingMetric(int campaingId, int metricId, DateTime date, double value)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var original = ctx.CampaingMetrics
                                  .Where(cm => (cm.CampaingId == campaingId) && (cm.MetricId == metricId) && (cm.Date == date))
                                  .FirstOrDefault();

                if (original != null)
                {
                    original.Value += value;
                }
                else
                {
                    ctx.CampaingMetrics.AddObject(new CampaingMetric { CampaingId = campaingId, MetricId = metricId, Date = date, Value = value });
                }

                ctx.SaveChanges();
            }
        }

        public int CreateProcessedFile(ProcessedFile file)
        {
            using (var ctx = new SelfManagementEntities())
            {
                ctx.ProcessedFiles.AddObject(file);
                ctx.SaveChanges();
            }

            return file.Id;
        }
    }
}
