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

        private IList<IDataFile> GetFilesToProcess(DateTime actDate)
        {
            var filesToProcess = new List<IDataFile>();
            var filter = "*" + String.Format("{0:yyyyMMdd}", actDate) + ".csv"; // Incomplete. This will only take QA, TTS and Summary files
            var paths = Directory.GetFiles(@"C:\ExternalFiles", filter);

            foreach (var path in paths)
            {
                var processedFile = this.metricsRepository.RetrieveProcessedFileByPath(path);

                if (processedFile == null)
                {
                    filesToProcess.Add(new DataFile(actDate, path));
                }
                else
                {
                    if (processedFile.HasErrors)
                    {
                        filesToProcess.Add(new DataFile(actDate, path));
                    }
                }
            }

            return filesToProcess;
        }

        private void Process()
        {
            var actDate = DateTime.Now;
            var filesToProcess = this.GetFilesToProcess(actDate);
            var avMetrics = this.metricsRepository.RetrieveAvailableMetrics();

            foreach (var metric in avMetrics)
            {
                var metricTypes = metric.CLRType.Split(',');
                IMetric metricProcessor = (IMetric)Activator.CreateInstance(metricTypes[1], metricTypes[0]);

                metricProcessor.ProcessFiles(filesToProcess); // TODO: Capture error event in metrics and log file error into DB
                var metricValues = metricProcessor.CalculatedValues;

                foreach (var legajo in metricValues.Keys)
                {
                    // TODO: Save metric values for the Agent in actual Campaing
                }

            }
        }
    }
}
