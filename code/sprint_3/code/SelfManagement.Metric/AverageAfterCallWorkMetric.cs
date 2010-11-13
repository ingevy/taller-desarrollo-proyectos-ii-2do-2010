﻿namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class AverageAfterCallWorkMetric : IMetric
    {
        public static double CalculateMetricValue(int tiempoEnAfterCallWorkMinutos, int cantLlamadas)
        {
            double result = Convert.ToDouble(tiempoEnAfterCallWorkMinutos) / Convert.ToDouble(cantLlamadas);
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

        public void ProcessFiles(IList<IDataFile> dataFiles)
        {
            var metricFiles = (from f in dataFiles
                               where f.ExternalSystemFile == this.externalFileNeeded
                               select f).ToList<IDataFile>();

            if (metricFiles.Count != 1)
            {
                throw new System.ArgumentException("Couldn't find necessary file to process metric"); 
            }

            this.metricDate = metricFiles.First().FileDate;

            var dataLines = metricFiles.First().DataLines;

            foreach (var line in dataLines)
            {
                var agentId = Convert.ToInt32(line["Legajo"]);
                var tiempoEnAfterCallWorkMinutos = Convert.ToInt32(line["Tiempo en after call work (min)"]);
                var cantLlamadas = Convert.ToInt32(line["Cantidad Llamadas"]);
                var metricValue = AverageAfterCallWorkMetric.CalculateMetricValue(tiempoEnAfterCallWorkMinutos, cantLlamadas);

                this.calculatedValues.Add(agentId, metricValue);
            }
        }
    }
}