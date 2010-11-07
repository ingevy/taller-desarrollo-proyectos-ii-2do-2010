namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class OverallQualityScorePercentMetric : IMetric
    {
        public static double CalculateMetricValue(int cantPuntosLogrados, int cantPuntosPosibles)
        {
            double result = (Convert.ToDouble(cantPuntosLogrados) / Convert.ToDouble(cantPuntosPosibles));
            return result;
        }
   
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private ExternalSystemFiles externalFileNeeded = ExternalSystemFiles.QA;
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
                var agentId = Convert.ToInt32(line["legajo"]);
                var cantPuntosPosibles = Convert.ToInt32(line["Cant de Puntos posibles"]);
                var cantPuntosLogrados = Convert.ToInt32(line["Cantidad Puntos logrados"]);
                var metricValue = OverallQualityScorePercentMetric.CalculateMetricValue(cantPuntosLogrados, cantPuntosPosibles);

                this.calculatedValues.Add(agentId, metricValue);
            }
        }
    }
}
