namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class TimeInAuxStatusMetric : IMetric
    {
        
        public TimeInAuxStatusMetric()
        {
            this.externalFilesNeeded.Add(ExternalSystemFiles.TTS);
            this.externalFilesNeeded.Add(ExternalSystemFiles.SUMMARY);
        }
        
        public static double CalculateMetricValue(DateTime fechaSalida, DateTime horarioSalida, DateTime fechaEntrada, DateTime horarioEntrada, int tiempoLoggeadoMinutos)
        {
            double result = 1000;
            return result;
        }
   
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private List<ExternalSystemFiles> externalFilesNeeded = new List<ExternalSystemFiles>(); 
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
                               from f2 in this.externalFilesNeeded
                               where f.ExternalSystemFile == f2
                                select f).ToList<IDataFile>();

            //Valido cantidad de archivos
            if (metricFiles.Count != 2)
            {
                throw new System.ArgumentException("Couldn't find necessaries files to process metric"); 
            }

            //Valido fechas de archivos
            if (metricFiles.ElementAt(0).FileDate != metricFiles.ElementAt(1).FileDate)
            {
                throw new System.ArgumentException("File Dates do not match"); 
            }
            
            this.metricDate = metricFiles.First().FileDate;

            IList<Dictionary<string, string>> dataLinesSummary = null;
            IList<Dictionary<string, string>> dataLinesTTS = null;

            switch (metricFiles.ElementAt(0).ExternalSystemFile)
            {
                case ExternalSystemFiles.SUMMARY:
                    dataLinesSummary = metricFiles.ElementAt(0).DataLines;
                    break;
                case ExternalSystemFiles.TTS:
                    dataLinesTTS = metricFiles.ElementAt(0).DataLines;
                    break;
                default:
                    break;
            }

            switch (metricFiles.ElementAt(1).ExternalSystemFile)
            {
                case ExternalSystemFiles.SUMMARY:
                    dataLinesSummary = metricFiles.ElementAt(1).DataLines;
                    break;
                case ExternalSystemFiles.TTS:
                    dataLinesTTS = metricFiles.ElementAt(1).DataLines;
                    break;
                default:
                    break;
            }
            

            foreach (var lineSummary in dataLinesSummary)
            {
                var agentIdSummary = Convert.ToInt32(lineSummary["Legajo"]);
                var tiempoLoggeadoMinutos = Convert.ToInt32(lineSummary["Tiempo Loggeado (min)"]);

                var lineTTS = (from l in dataLinesTTS
                               where agentIdSummary == Convert.ToInt32(l["legajo"])
                               select l).ToList<Dictionary<string, string>>();


                if (lineTTS.Count != 1)
                {
                    throw new System.ArgumentException("The agentID " + agentIdSummary + " in Summary File, was not found in TTS File");
                }

                var fechaSalida = DateTime.ParseExact(lineTTS.First()["fecha Salida"], "dd/MM/yyyy", null);
                var horarioSalida = DateTime.ParseExact(lineTTS.First()["Horario Salida"], "HH:mm", null);
                var fechaEntrada = DateTime.ParseExact(lineTTS.First()["fecha Entrada"], "dd/MM/yyyy", null);
                var horarioEntrada = DateTime.ParseExact(lineTTS.First()["Horario Entrada"], "HH:mm", null);
                
                var metricValue = TimeInAuxStatusMetric.CalculateMetricValue(fechaSalida, horarioSalida, fechaEntrada, horarioEntrada, tiempoLoggeadoMinutos);

                this.calculatedValues.Add(agentIdSummary, metricValue);
            }
            
        }
    }
}
