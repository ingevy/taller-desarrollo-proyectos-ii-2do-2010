namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CallCenter.SelfManagement.Data.Helpers;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class MetricsRepository : IMetricsRepository
    {
        public ProcessedFile RetrieveProcessedFilesById(int fileId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.ProcessedFiles.FirstOrDefault(pf => pf.Id == fileId);
            }
        }

        public ProcessedFile RetrieveProcessedFileByPath(string filePath)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.ProcessedFiles.FirstOrDefault(pf => pf.FileSystemPath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            }
        }

        public IList<ProcessedFile> FilterProcessedFiles(string dataDate, string processingDate, string modifiedDate, int? type, int? state)
        {
            using (var ctx = new SelfManagementEntities())
            {
                IQueryable<ProcessedFile> filter = ctx.ProcessedFiles;
                int number;

                if (type.HasValue)
                {
                    filter = filter.Where(f => f.FileType == type.Value);
                }

                if (state.HasValue)
                {
                    filter = filter.Where(f => f.HasErrors == (state.Value != 0));
                }

                DateTime dateProcessed;
                if (DateTime.TryParseExact(processingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateProcessed))
                {
                    filter = filter.Where(f => (f.DateProcessed.Year == dateProcessed.Year) && (f.DateProcessed.Month == dateProcessed.Month) && (f.DateProcessed.Day == dateProcessed.Day));
                }

                DateTime dateLastModified;
                if (DateTime.TryParseExact(modifiedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateLastModified))
                {
                    filter = filter.Where(f => (f.DateLastModified.Year == dateLastModified.Year) && (f.DateLastModified.Month == dateLastModified.Month) && (f.DateLastModified.Day == dateLastModified.Day));
                }

                DateTime dateData;
                if (DateTime.TryParseExact(dataDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateData))
                {
                    return filter
                            .ToList()
                            .Where(f => IsInDateData(f, dateData))
                            .ToList();
                }
                else
                {
                    return filter.ToList();
                }
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

        public int RetrieveUserCampaingId(int innerUserId, DateTime date)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Campaings
                        .Where(c => ctx.CampaingUsers.Any(cu => (cu.CampaingId == c.Id) && (cu.InnerUserId == innerUserId)
                                                          && (cu.BeginDate <= date) && (cu.EndDate >= date)))
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

        public MonthlySchedule RetrieveAgentMonthlySchedule(int innerUserId, short year, byte month)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return (from ms in ctx.MonthlySchedules
                        where ms.InnerUserId == innerUserId && ms.Year == year && ms.Month == month
                        select ms).FirstOrDefault();
            }
        }

        public bool IsHolidayDate(DateTime date)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var cDate = new DateTime(date.Year, date.Month, date.Day);
                return ((from h in ctx.Holidays
                         where h.Date == cDate
                         select h).Count() == 1);
            }
        }

        public void SaveOrUpdateMonthlySchedule(MonthlySchedule schedule)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var original = (from ms in ctx.MonthlySchedules
                                where ms.InnerUserId == schedule.InnerUserId && ms.Year == schedule.Year && ms.Month == schedule.Month
                                select ms).FirstOrDefault();

                if (original == null)
                {
                    ctx.MonthlySchedules.AddObject(schedule);
                }
                else
                {
                    original.TotalHoursWorked = schedule.TotalHoursWorked;
                    original.ExtraHoursWorked50 = schedule.ExtraHoursWorked50;
                    original.ExtraHoursWorked100 = schedule.ExtraHoursWorked100;
                    original.LastDayModified = schedule.LastDayModified;
                }

                ctx.SaveChanges();
            }
        }

        public double GetUserMetricValue(int innerUserId, DateTime date, int metricId, int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var campaing = ctx.Campaings.FirstOrDefault(c => c.Id == campaingId);
                var metric = ctx.Metrics.FirstOrDefault(m => m.Id == metricId);

                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                var lowLimitDate = (campaing.BeginDate < firstDayOfMonth) ? firstDayOfMonth : campaing.BeginDate;

                var userMetrics = (from m in ctx.UserMetrics
                                   where m.InnerUserId == innerUserId && m.Date >= lowLimitDate && m.Date <= lastDayOfMonth
                                         && m.MetricId == metricId && m.CampaingId == campaing.Id
                                   select m).ToList();
                
                var metricValue = 0.0;

                if ((metric.Format == 0) || (metric.Format == 2))
                {
                    metricValue = MetricsCalculator.CalculateAverageMetricValue(userMetrics, date, metric.Format);
                }
                else
                {
                    metricValue = MetricsCalculator.CalculateAcumulatedMetricValue(userMetrics, date);
                }

                return metricValue;
            }
        }

        public double GetCampaingMetricValue(int campaingId, DateTime date, int metricId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var campaing = ctx.Campaings.FirstOrDefault(c => c.Id == campaingId);
                var metric = ctx.Metrics.FirstOrDefault(m => m.Id == metricId);

                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                var lowLimitDate = (campaing.BeginDate < firstDayOfMonth) ? firstDayOfMonth : campaing.BeginDate;

                var campaingMetrics = (from cm in ctx.CampaingMetrics
                                   where cm.Date >= lowLimitDate && cm.Date <= lastDayOfMonth
                                         && cm.MetricId == metricId && cm.CampaingId == campaing.Id
                                   select cm).ToList();

                var metricValue = 0.0;

                if ((metric.Format == 0) || (metric.Format == 2))
                {
                    metricValue = MetricsCalculator.CalculateAverageMetricValue(campaingMetrics, date, metric.Format);
                }
                else
                {
                    metricValue = MetricsCalculator.CalculateAcumulatedMetricValue(campaingMetrics, date);
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

        public void CreateSupervisorAgent(SupervisorAgent supervisorAgent)
        {
            using (var ctx = new SelfManagementEntities())
            {
                ctx.SupervisorAgents.AddObject(supervisorAgent);
                ctx.SaveChanges();
            }
        }

        public void CreateOrUpdateSupervisorMetric(int metricId, DateTime date)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var supervisors = ctx.SupervisorAgents
                                  .Where(s => ctx.UserMetrics.Any(um => (um.MetricId == metricId) && (um.Date == date) && (um.InnerUserId == s.AgentId)))
                                  .Select(s => s.SupervisorId).Distinct().ToList();

                foreach (var supervisorId in supervisors)
                {
                    /*var supervisorCampaingId = ctx.Campaings
                                               .Where(c => ctx.CampaingUsers.Any(cu => (cu.CampaingId == c.Id) && (cu.InnerUserId == supervisorId)
                                                      && (cu.BeginDate <= date) && (cu.EndDate >= date)))
                                               .FirstOrDefault().Id;*/

                    var supervisorCampaingId = ctx.CampaingUsers
                                               .Where(cu => (cu.InnerUserId == supervisorId) && (cu.BeginDate <= date) && (cu.EndDate >= date))
                                               .FirstOrDefault().CampaingId;

                    var agentsMetric = ctx.UserMetrics
                                       .Where(um => (um.MetricId == metricId) && (um.Date == date) && (um.CampaingId == supervisorCampaingId)
                                              && ctx.SupervisorAgents.Any(sa => (sa.AgentId == um.InnerUserId) && (sa.SupervisorId == supervisorId)))
                                       .ToList();

                    var supervisorMetricValue = 0.0;

                    if (agentsMetric.Count > 0)
                    {
                        foreach (var am in agentsMetric)
                        {
                            supervisorMetricValue += am.Value;
                        }

                        supervisorMetricValue = supervisorMetricValue / Convert.ToDouble(agentsMetric.Count);

                        var supervisorMetric = ctx.UserMetrics
                                               .Where(um => (um.MetricId == metricId) && (um.Date == date)
                                                      && (um.InnerUserId == supervisorId) && (um.CampaingId == supervisorCampaingId))
                                               .FirstOrDefault();

                        if (supervisorMetric != null)
                        {
                            supervisorMetric.Value = supervisorMetricValue;
                        }
                        else
                        {
                            ctx.UserMetrics.AddObject(new UserMetric
                            {
                                InnerUserId = supervisorId,
                                CampaingId = supervisorCampaingId,
                                MetricId = metricId,
                                Date = date,
                                Value = supervisorMetricValue
                            });
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }

        public void CreateOrUpdateCampaingMetric(int metricId, DateTime date)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var campaings = ctx.CampaingUsers
                                .Where(cu => ctx.UserMetrics.Any(um => (um.MetricId == metricId) && (um.Date == date) && (um.InnerUserId == cu.InnerUserId)))
                                .Select(cu => cu.CampaingId).Distinct().ToList();

                foreach (var campaingId in campaings)
                {
                    var agentsMetric = ctx.UserMetrics
                                       .Where(um => (um.MetricId == metricId) && (um.Date == date) && (um.CampaingId == campaingId)
                                              && (!ctx.Supervisors.Any(s => s.InnerUserId == um.InnerUserId)))
                                       .ToList();

                    var campaingMetricValue = 0.0;

                    if (agentsMetric.Count > 0)
                    {
                        foreach (var am in agentsMetric)
                        {
                            campaingMetricValue += am.Value;
                        }

                        campaingMetricValue = campaingMetricValue / Convert.ToDouble(agentsMetric.Count);

                        var campaingMetric = ctx.CampaingMetrics
                                               .Where(cm => (cm.MetricId == metricId) && (cm.Date == date) && (cm.CampaingId == campaingId))
                                               .FirstOrDefault();

                        if (campaingMetric != null)
                        {
                            campaingMetric.Value = campaingMetricValue;
                        }
                        else
                        {
                            ctx.CampaingMetrics.AddObject(new CampaingMetric
                            {
                                CampaingId = campaingId,
                                MetricId = metricId,
                                Date = date,
                                Value = campaingMetricValue
                            });
                        }
                    }
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

        public void CleanProcessedFile(ProcessedFile file)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var processedFile = (from f in ctx.ProcessedFiles
                                     where f.Id == file.Id
                                     select f).ToList().FirstOrDefault();
                
                processedFile.Log = "";
                processedFile.HasErrors = false;

                ctx.SaveChanges();
            }
        }

        public void LogInProcessedFile(string filePath, string logMessage)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var processedFile = ctx.ProcessedFiles
                                       .Where(pf => pf.FileSystemPath == filePath)
                                       .FirstOrDefault();

                if (processedFile != null)
                {
                    processedFile.Log = (processedFile.Log == "") ? logMessage : processedFile.Log + "\n" + logMessage;
                    processedFile.HasErrors = true;
                }

                ctx.SaveChanges();
            }
        }

        public void ChangeAgentSupervisor(int agentId, int newSupervisorId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var supervisorAsignment = (from sa in ctx.SupervisorAgents
                                           where sa.AgentId == agentId
                                           select sa).ToList().FirstOrDefault();

                supervisorAsignment.SupervisorId = newSupervisorId;

                ctx.SaveChanges();
            }
        }

        public void ChangeAgentSupervisorAndCampaing(int agentId, int newSupervisorId, int newCampaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                var actualAgentCampaing = ctx.Campaings
                                          .Where(c => ctx.CampaingUsers.Any(cu => (cu.CampaingId == c.Id) && (cu.InnerUserId == agentId)
                                                          && (cu.BeginDate <= actualDate) && (cu.EndDate >= actualDate)))
                                          .FirstOrDefault();

                var newCampaing = ctx.Campaings
                                  .Where(c => c.Id == newCampaingId)
                                  .FirstOrDefault();

                var beginDate = (newCampaing.BeginDate > actualDate) ? newCampaing.BeginDate : actualDate;
                var endDate = newCampaing.EndDate.GetValueOrDefault(actualDate);

                if (actualAgentCampaing == null)
                {
                    var campaingUser = new CampaingUser { CampaingId = newCampaing.Id, InnerUserId = agentId, BeginDate = beginDate, EndDate = endDate };

                    ctx.CampaingUsers.AddObject(campaingUser);
                }
                else
                {
                    var oldCampaingUser = ctx.CampaingUsers
                                          .Where(cu => (cu.CampaingId == actualAgentCampaing.Id) && (cu.InnerUserId == agentId)
                                                        && (cu.BeginDate <= actualDate) && (cu.EndDate >= actualDate))
                                          .FirstOrDefault();

                    oldCampaingUser.EndDate = actualDate.AddDays(-1.0);

                    var campaingUser = new CampaingUser { CampaingId = newCampaing.Id, InnerUserId = agentId, BeginDate = beginDate, EndDate = endDate };

                    ctx.CampaingUsers.AddObject(campaingUser);
                }

                var supervisorAsignment = (from sa in ctx.SupervisorAgents
                                           where sa.AgentId == agentId
                                           select sa).ToList().FirstOrDefault();

                supervisorAsignment.SupervisorId = newSupervisorId;

                ctx.SaveChanges();
            }
        }

        private static bool IsInDateData(ProcessedFile file, DateTime dateData)
        {
            if (((ExternalSystemFiles)file.FileType) == ExternalSystemFiles.HF)
            {
                return true;
            }

            var fileDateDate = DateTime.ParseExact(Path.GetFileNameWithoutExtension(file.FileSystemPath).Split('_')[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            return fileDateDate.Date == dateData.Date;
        }
    }
}
