﻿namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Helpers;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class TimeInAuxStatusMetric : IMetric
    {        
        public TimeInAuxStatusMetric()
        {
            this.externalFilesNeeded.Add(ExternalSystemFiles.TTS);
            this.externalFilesNeeded.Add(ExternalSystemFiles.SUMMARY);
        }
        
        public static double CalculateMetricValue(DateTime dateTimeSalida, DateTime dateTimeEntrada, int tiempoLoggeadoMinutos)
        {
            double result = -1; 
            
            TimeSpan span = dateTimeSalida.Subtract ( dateTimeEntrada );

            result = span.TotalMinutes - Convert.ToDouble(tiempoLoggeadoMinutos);
             
            return result;
        }
   
        private IDictionary<int, double> calculatedValues = new Dictionary<int, double>();
        private IList<ExternalSystemFiles> externalFilesNeeded = new List<ExternalSystemFiles>(); 
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
                return this.externalFilesNeeded;
            }
        }

        public void ProcessFiles(IList<IDataFile> dataFiles)
        {
            var metricFiles = (from f in dataFiles
                               from f2 in this.externalFilesNeeded
                               where f.ExternalSystemFile == f2
                                select f).ToList<IDataFile>();

            // Valido cantidad de archivos
            if (metricFiles.Count != 2)
            {
                throw new System.ArgumentException("No se encontraron los archivos necesarios para procesar la metrica"); 
            }

            // Valido fechas de archivos
            if (metricFiles.ElementAt(0).FileDate != metricFiles.ElementAt(1).FileDate)
            {
                throw new System.ArgumentException("La fecha de los archivos no coincide"); 
            }

            try
            {
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
                    try
                    {
                        var agentIdSummary = Convert.ToInt32(lineSummary["Legajo"]);
                        var tiempoLoggeadoMinutos = Convert.ToInt32(lineSummary["Tiempo Loggeado (min)"]);

                        var lineTTS = (from l in dataLinesTTS
                                       where agentIdSummary == Convert.ToInt32(l["legajo"])
                                       select l).ToList<Dictionary<string, string>>();


                        if (lineTTS.Count != 1)
                        {
                            throw new ArgumentException("El Agente " + agentIdSummary + " del archivo Summary, no se encontro en el archivo TTS");
                        }

                        // Parseo fechas
                        Int32[] fechaSalida = ParseDates(lineTTS.First(), "fecha Salida", '/', agentIdSummary);
                        Int32 diaSalida = Convert.ToInt32(fechaSalida[0]);
                        Int32 mesSalida = Convert.ToInt32(fechaSalida[1]);
                        Int32 anioSalida = Convert.ToInt32(fechaSalida[2]);
                        Int32[] horarioSalida = ParseSchedules(lineTTS.First(), "Horario Salida", ':', agentIdSummary);
                        Int32 horaSalida = Convert.ToInt32(horarioSalida[0]);
                        Int32 minutosSalida = Convert.ToInt32(horarioSalida[1]);

                        DateTime dateTimeSalida = new DateTime(anioSalida, mesSalida, diaSalida, horaSalida, minutosSalida, Convert.ToInt32(0));

                        Int32[] fechaEntrada = ParseDates(lineTTS.First(), "fecha Entrada", '/', agentIdSummary);
                        Int32 diaEntrada = Convert.ToInt32(fechaEntrada[0]);
                        Int32 mesEntrada = Convert.ToInt32(fechaEntrada[1]);
                        Int32 anioEntrada = Convert.ToInt32(fechaEntrada[2]);
                        Int32[] horarioEntrada = ParseSchedules(lineTTS.First(), "Horario Entrada", ':', agentIdSummary);
                        Int32 horaEntrada = Convert.ToInt32(horarioEntrada[0]);
                        Int32 minutosEntrada = Convert.ToInt32(horarioEntrada[1]);

                        DateTime dateTimeEntrada = new DateTime(anioEntrada, mesEntrada, diaEntrada, horaEntrada, minutosEntrada, Convert.ToInt32(0));

                        var metricValue = TimeInAuxStatusMetric.CalculateMetricValue(dateTimeSalida, dateTimeEntrada, tiempoLoggeadoMinutos);

                        this.calculatedValues.Add(agentIdSummary, metricValue);
                    }
                    catch (Exception e)
                    {
                        throw new MetricException("Summary "+this.MetricDate.ToString("dd-MM-yyyy")+" Linea " + (dataLinesSummary.IndexOf(lineSummary) + 1) + ": " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new MetricException(e.Message);
            }
        }

        /*
         * Retorna Int32[] con dia, mes, anio en este orden
         * 
         */
        public static Int32[] ParseDates(Dictionary<string,string> lineTTS, string nombreCampo, char separador, int agentId)
        {
            Int32[] result = new Int32[3];

            string[] dateStr = null;
            dateStr = lineTTS[nombreCampo].Split(separador);

            if (dateStr.Length != 3)
            {
                throw new System.ArgumentException("Formato " + nombreCampo + " para agente " + agentId + " distinto a dd/MM/yyyy en archivo TTS.");
            }
                        
            result[0] = Convert.ToInt32(dateStr[0]); //dia
            result[1] = Convert.ToInt32(dateStr[1]); //mes
            result[2] = Convert.ToInt32(dateStr[2]); //anio
            
            return result;
        }

        /*
         * Retorna Int32[] con hora, minutos en este orden
         * 
         */
        public static Int32[] ParseSchedules(Dictionary<string, string> lineTTS, string nombreCampo, char separador, int agentId)
        {
            Int32[] result = new Int32[3];

            string[] dateStr = null;
            dateStr = lineTTS[nombreCampo].Split(separador);

            if (dateStr.Length != 2)
            {
                throw new System.ArgumentException("Formato " + nombreCampo + " para agente " + agentId + " distinto a HH:mm en archivo TTS.");
            }

            result[0] = Convert.ToInt32(dateStr[0]); //hora
            result[1] = Convert.ToInt32(dateStr[1]); //minutos

            return result;
        }    
    }
}
