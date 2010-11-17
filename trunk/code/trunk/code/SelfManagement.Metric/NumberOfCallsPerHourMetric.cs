namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Helpers;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class NumberOfCallsPerHourMetric : IMetric
    {
        public static double CalculateMetricValue(int cantLlamadas, int tiempoLoggeadoMinutos)
        {
            double result = ((Convert.ToDouble(cantLlamadas) / Convert.ToDouble(tiempoLoggeadoMinutos)) * 60); //60 min/hora
            return result;
        }
   
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private ExternalSystemFiles externalFileNeeded = ExternalSystemFiles.SUMMARY;
        private DateTime metricDate;

        public IDictionary<int, double> CalculatedValues
        {
            get { return this.calculatedValues; }
        }

        public DateTime MetricDate
        {
            get { return this.metricDate; }
        }

        public IList<ExternalSystemFiles> ExternalFilesNeeded
        {
            get
            {
                var files = new List<ExternalSystemFiles>();
                files.Add(this.externalFileNeeded);

                return files;
            }
        }

        public void ProcessFiles(IList<IDataFile> dataFiles)
        {
            var metricFiles = (from f in dataFiles
                               where f.ExternalSystemFile == this.externalFileNeeded
                               select f).ToList<IDataFile>();

            if (metricFiles.Count != 1)
            {
                throw new System.ArgumentException("Couldn't find necessary file to process metric"); 
            }

            try
            {
                this.metricDate = metricFiles.First().FileDate;

                var dataLines = metricFiles.First().DataLines;

                foreach (var line in dataLines)
                {
                    try
                    {
                        var agentId = Convert.ToInt32(line["Legajo"]);
                        var cantLlamadas = Convert.ToInt32(line["Cantidad Llamadas"]);
                        var tiempoLoggeadoMinutos = Convert.ToInt32(line["Tiempo Loggeado (min)"]);
                        var metricValue = NumberOfCallsPerHourMetric.CalculateMetricValue(cantLlamadas, tiempoLoggeadoMinutos);

                        this.calculatedValues.Add(agentId, metricValue);
                    }
                    catch (Exception e)
                    {
                        throw new MetricException("Linea " + (dataLines.IndexOf(line) + 1) + ": " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new MetricException(e.Message);
            }
        }
    }
}
