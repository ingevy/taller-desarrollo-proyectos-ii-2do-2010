namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class PercentageOfTimeSpentInBillableModeMetric : IMetric
    {
        public static double CalculateMetricValue(int tiempoInCallMinutos, int tiempoEnEsperaMinutos, int tiempoEnAfterCallWorkMinutos, int tiempoLoggeadoMinutos)
        {
            double result = (Convert.ToDouble(tiempoInCallMinutos) + Convert.ToDouble(tiempoEnEsperaMinutos) + Convert.ToDouble(tiempoEnAfterCallWorkMinutos)) / Convert.ToDouble(tiempoLoggeadoMinutos);
            return result;
        }
   
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private ExternalSystemFiles externalFileNeeded = ExternalSystemFiles.SUMMARY;
        private DateTime metricDate;

        public PercentageOfTimeSpentInBillableModeMetric(DateTime metricDate)
        {
            this.metricDate = metricDate;
        }

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

            if (metricFiles.First().FileDate != this.MetricDate)
            {
                throw new System.ArgumentException("File date does not match Metric date"); 
            }

            var dataLines = metricFiles.First().DataLines;

            foreach (var line in dataLines)
            {
                var agentId = Convert.ToInt32(line["Legajo"]);
                var tiempoInCallMinutos = Convert.ToInt32(line["Tiempo InCall (min)"]);
                var tiempoEnEsperaMinutos = Convert.ToInt32(line["Tiempo en espera (min)"]);
                var tiempoEnAfterCallWorkMinutos = Convert.ToInt32(line["Tiempo en after call work (min)"]);
                var tiempoLoggeadoMinutos = Convert.ToInt32(line["Tiempo Loggeado (min)"]);
                var metricValue = PercentageOfTimeSpentInBillableModeMetric.CalculateMetricValue(tiempoInCallMinutos, tiempoEnEsperaMinutos, tiempoEnAfterCallWorkMinutos, tiempoLoggeadoMinutos);

                this.calculatedValues.Add(agentId, metricValue);
            }
        }
    }
}
