﻿namespace CallCenter.SelfManagement.Metric
{
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using System;
    using System.Globalization;

    public class InteractionToCallPercentMetric : IMetric
    {
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private string valueType = "Percent";
        private DateTime metricDate;

        public InteractionToCallPercentMetric(DateTime metricDate)
        {
            this.metricDate = metricDate;
        }

        public IDictionary<int, double> CalculatedValues
        {
            get { return this.calculatedValues; }
        }

        public string ValueType
        {
            get { return this.valueType; }
        }

        public DateTime MetricDate
        {
            get { return this.metricDate; }
        }

        public void ProcessFiles(IList<IDataFile> dataFiles)
        {
            var metricFiles = (from f in dataFiles
                               where f.ExternalSystemName == "Summary"
                               select f).ToList<IDataFile>();

            if (metricFiles.Count != 1)
            { 
                throw new System.ArgumentException("Couldn't find necessary file to process metric"); 
            }

            if (metricFiles.First().FileDate != this.MetricDate)
            {
                throw new System.ArgumentException("File date does not match Metric date"); 
            }

            var dataLines = metricFiles.First().DataLines;

            foreach (var line in dataLines)
            {
                //var lineDate = Convert.ToDateTime(line[1],new CultureInfo("es-AR"));
                var agentId = Convert.ToInt32(line[0]);
                var cantLlamadas = Convert.ToInt32(line[2]);
                var cantTransferidas = Convert.ToInt32(line[5]);
                var metricValue = (cantLlamadas - cantTransferidas) / cantLlamadas;

                this.calculatedValues.Add(agentId, metricValue);
            }
        }
    }
}
