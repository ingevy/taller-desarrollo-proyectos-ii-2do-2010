namespace CallCenter.SelfManagement.FilesProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            var paths = Directory.GetFiles(Environment.CurrentDirectory, filter);

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

        public void Process()
        {
            var filesToProcess = this.GetFilesToProcess();

            if (filesToProcess.Count > 0)
            {
                var avMetrics = this.metricsRepository.RetrieveAvailableMetrics();

                foreach (var metric in avMetrics)
                {
                    var metricTypes = metric.CLRType.Split(',');
                    IMetric metricProcessor = (IMetric)Activator.CreateInstance(metricTypes[1], metricTypes[0]);

                    metricProcessor.ProcessFiles(filesToProcess); // TODO: Capture error event in metrics and log file error into DB
                    var metricValues = metricProcessor.CalculatedValues;

                    foreach (var legajo in metricValues.Keys)
                    {
                        Console.WriteLine("Metrica: " + metric.MetricName);
                        Console.WriteLine(" Agente: " + legajo);
                        Console.WriteLine("     Fecha: " + metricProcessor.MetricDate);
                        Console.WriteLine("     Valor: " + metricValues[legajo]);

                        /*var agentCampaing = this.metricsRepository.RetrieveAgentActualCampaing(legajo);
                        var userMetric = new UserMetric();
                        userMetric.CampaingId = agentCampaing.Id;
                        userMetric.InnerUserId = legajo;
                        userMetric.MetricId = metric.Id;
                        userMetric.Date = metricProcessor.MetricDate;
                        userMetric.Value = metricValues[legajo];
                        this.metricsRepository.SaveUserMetric(userMetric);
                        this.metricsRepository.SaveOrUpdateCampaingMetric(agentCampaing.Id, metric.Id, metricProcessor.MetricDate, metricValues[legajo]);*/

                    }
                }
            }
        }
    }
}
