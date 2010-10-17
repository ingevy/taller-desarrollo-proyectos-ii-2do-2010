namespace CallCenter.SelfManagement.FilesProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Metric;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class FilesProcessor
    {
        private readonly IMetricsRepository metricsRepository;

        public FilesProcessor(IMetricsRepository metricsRepository)
        {
            this.metricsRepository = metricsRepository;
        }

        private IList<IDataFile> GetFilesToProcess()
        {
            var filesToProcess = new List<IDataFile>();
            var filter = "*.csv";
            var paths = Directory.GetFiles(ConfigurationManager.AppSettings["ExternalFilesLocation"].ToString(), filter);

            foreach (var path in paths)
            {
                var processedFile = this.metricsRepository.RetrieveProcessedFileByPath(path);

                if (processedFile == null)
                {
                    filesToProcess.Add(new DataFile(path));
                }
                else
                {
                    if (processedFile.HasErrors)
                    {
                        if (File.GetLastWriteTime(path) != processedFile.DateLastModified)
                        {
                            filesToProcess.Add(new DataFile(path));
                        }
                    }
                }
            }

            return filesToProcess;
        }

        private Dictionary<DateTime,IList<IDataFile>> GroupFilesToProcessByDate(IList<IDataFile> filesToProcess)
        {
            var groupedFiles = new Dictionary<DateTime, IList<IDataFile>>();

            foreach (var file in filesToProcess)
            {
                groupedFiles[file.FileDate] = new List<IDataFile>();
            }

            foreach (var file in filesToProcess)
            {
                groupedFiles[file.FileDate].Add(file);
            }

            return groupedFiles;
        }

        public void ProcessMetrics()
        {
            Console.WriteLine("Procesando Metricas...");

            var filesToProcess = this.GetFilesToProcess();

            if (filesToProcess.Count > 0)
            {
                var groupedFiles = this.GroupFilesToProcessByDate(filesToProcess);

                foreach (var date in groupedFiles.Keys)
                {
                    foreach (var processedFile in groupedFiles[date])
                    {
                        this.metricsRepository.CreateProcessedFile(new ProcessedFile
                        {
                            FileSystemPath = processedFile.FilePath,
                            FileType = (int)processedFile.ExternalSystemFile,
                            DateProcessed = DateTime.Now,
                            DateLastModified = File.GetLastWriteTime(processedFile.FilePath),
                            Log = "",
                            HasErrors = false
                        });
                    }

                    var avMetrics = this.metricsRepository.RetrieveAvailableMetrics();

                    foreach (var metric in avMetrics)
                    {
                        var metricTypes = metric.CLRType.Split(',');
                        IMetric metricProcessor = (IMetric)Activator.CreateInstance(metricTypes[1], metricTypes[0]).Unwrap();

                        try
                        {
                            metricProcessor.ProcessFiles(groupedFiles[date]); // TODO: Capture error event in metrics and log file error into DB
                            var metricValues = metricProcessor.CalculatedValues;

                            Console.WriteLine("Grabando Metrica: " + metric.MetricName + " - Fecha: " + metricProcessor.MetricDate.ToString("dd/MM/yyyy"));
                            foreach (var legajo in metricValues.Keys)
                            {
                                var agentCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(legajo);
                                var agentSupervisorId = this.metricsRepository.RetrieveAgentSupervisorId(legajo);
                                var supervisorCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(agentSupervisorId);
                                if (agentCampaingId != supervisorCampaingId)
                                {
                                    throw new ApplicationException("Data Inconsistency");
                                }
                                var userMetric = new UserMetric
                                {
                                    CampaingId = agentCampaingId,
                                    InnerUserId = legajo,
                                    MetricId = metric.Id,
                                    Date = metricProcessor.MetricDate,
                                    Value = metricValues[legajo]
                                };
                                this.metricsRepository.CreateAgentMetric(userMetric);
                                this.metricsRepository.CreateOrUpdateSupervisorMetric(agentSupervisorId, agentCampaingId, metric.Id, metricProcessor.MetricDate, metricValues[legajo]);
                                this.metricsRepository.CreateOrUpdateCampaingMetric(agentCampaingId, metric.Id, metricProcessor.MetricDate, metricValues[legajo]);
                            }
                        }
                        catch
                        {
                            // To bypass the metrics throw generated by the HF file taken from 'nuestros' dir
                        }
                    }
                }
            }
        }

        public void ProcessHumanForceFile()
        {
            var filesToProcess = this.GetFilesToProcess();

            var hfFile = (from f in filesToProcess
                          where f.ExternalSystemFile == ExternalSystemFiles.HF
                          select f).ToList<IDataFile>();

            if (filesToProcess.Count == 1)
            {
                var dataLines = filesToProcess[0].DataLines;

                foreach (var line in dataLines)
                {
                    
                    /*var agentId = Convert.ToInt32(line["Legajo"]);
                    var cantLlamadas = Convert.ToInt32(line["Cantidad Llamadas"]);
                    var cantTransferidas = Convert.ToInt32(line["Cantidad Llamadas Transferidas"]);
                    var metricValue = InteractionToCallPercentMetric.CalculateMetricValue(cantLlamadas, cantTransferidas);

                    calculatedValues.Add(agentId, metricValue);*/
                }
            }
        }
    }
}
