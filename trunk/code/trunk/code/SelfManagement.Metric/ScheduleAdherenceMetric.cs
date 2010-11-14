namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class ScheduleAdherenceMetric : IMetric
    {
        
        public ScheduleAdherenceMetric()
        {
            this.externalFilesNeeded.Add(ExternalSystemFiles.STS);
            this.externalFilesNeeded.Add(ExternalSystemFiles.TTS);
        }

        public static double CalculateMetricValue(DateTime dateTimeEntradaSTS, DateTime dateTimeEntradaTTS, DateTime dateTimeSalidaSTS, DateTime dateTimeSalidaTTS)
        {
            double result = 0;

            //Veo si entro mas tarde de lo planificado
            TimeSpan spanEntrada = dateTimeEntradaTTS.Subtract(dateTimeEntradaSTS);
            //Veo si se fue antes de lo planificado
            TimeSpan spanSalida = dateTimeSalidaSTS.Subtract( dateTimeSalidaTTS );

            //Sumo solo si entro mas tarde o si salio mas temprano
            if (spanEntrada.TotalMilliseconds > 0)
                result += spanEntrada.TotalMinutes;

            if (spanSalida.TotalMilliseconds > 0)
                result += spanSalida.TotalMinutes;
 
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

            IList<Dictionary<string, string>> dataLinesSTS = null;
            IList<Dictionary<string, string>> dataLinesTTS = null;

            switch (metricFiles.ElementAt(0).ExternalSystemFile)
            {
                case ExternalSystemFiles.STS:
                    dataLinesSTS = metricFiles.ElementAt(0).DataLines;
                    break;
                case ExternalSystemFiles.TTS:
                    dataLinesTTS = metricFiles.ElementAt(0).DataLines;
                    break;
                default:
                    break;
            }

            switch (metricFiles.ElementAt(1).ExternalSystemFile)
            {
                case ExternalSystemFiles.STS:
                    dataLinesSTS = metricFiles.ElementAt(1).DataLines;
                    break;
                case ExternalSystemFiles.TTS:
                    dataLinesTTS = metricFiles.ElementAt(1).DataLines;
                    break;
                default:
                    break;
            }
            

            foreach (var lineSTS in dataLinesSTS)
            {
                var agentIdSTS = Convert.ToInt32(lineSTS["legajo"]);
                //Parseo fechas de STS
                Int32[] fechaSalidaSTS = parseFechas(lineSTS, "fecha Salida", '/', agentIdSTS, "STS");
                Int32 diaSalidaSTS = Convert.ToInt32(fechaSalidaSTS[0]);
                Int32 mesSalidaSTS = Convert.ToInt32(fechaSalidaSTS[1]);
                Int32 anioSalidaSTS = Convert.ToInt32(fechaSalidaSTS[2]);
                Int32[] horarioSalidaSTS = parseHorarios(lineSTS, "Horario Salida", ':', agentIdSTS, "STS");
                Int32 horaSalidaSTS = Convert.ToInt32(horarioSalidaSTS[0]);
                Int32 minutosSalidaSTS = Convert.ToInt32(horarioSalidaSTS[1]);

                DateTime dateTimeSalidaSTS = new DateTime(anioSalidaSTS, mesSalidaSTS, diaSalidaSTS, horaSalidaSTS, minutosSalidaSTS, Convert.ToInt32(0));

                Int32[] fechaEntradaSTS = parseFechas(lineSTS, "fecha Entrada", '/', agentIdSTS, "STS");
                Int32 diaEntradaSTS = Convert.ToInt32(fechaEntradaSTS[0]);
                Int32 mesEntradaSTS = Convert.ToInt32(fechaEntradaSTS[1]);
                Int32 anioEntradaSTS = Convert.ToInt32(fechaEntradaSTS[2]);
                Int32[] horarioEntradaSTS = parseHorarios(lineSTS, "Horario Entrada", ':', agentIdSTS, "STS");
                Int32 horaEntradaSTS = Convert.ToInt32(horarioEntradaSTS[0]);
                Int32 minutosEntradaSTS = Convert.ToInt32(horarioEntradaSTS[1]);

                DateTime dateTimeEntradaSTS = new DateTime(anioEntradaSTS, mesEntradaSTS, diaEntradaSTS, horaEntradaSTS, minutosEntradaSTS, Convert.ToInt32(0));



                var lineTTS = (from l in dataLinesTTS
                               where agentIdSTS == Convert.ToInt32(l["legajo"])
                               select l).ToList<Dictionary<string, string>>();


                if (lineTTS.Count == 1)
                {
                    //Parseo fechas
                    Int32[] fechaSalidaTTS = parseFechas(lineTTS.First(), "fecha Salida", '/', agentIdSTS, "TTS"); //agentIdSTS es igual a agentIdTTS
                    Int32 diaSalidaTTS = Convert.ToInt32(fechaSalidaTTS[0]);
                    Int32 mesSalidaTTS = Convert.ToInt32(fechaSalidaTTS[1]);
                    Int32 anioSalidaTTS = Convert.ToInt32(fechaSalidaTTS[2]);
                    Int32[] horarioSalidaTTS = parseHorarios(lineTTS.First(), "Horario Salida", ':', agentIdSTS, "TTS"); //agentIdSTS es igual a agentIdTTS
                    Int32 horaSalidaTTS = Convert.ToInt32(horarioSalidaTTS[0]);
                    Int32 minutosSalidaTTS = Convert.ToInt32(horarioSalidaTTS[1]);

                    DateTime dateTimeSalidaTTS = new DateTime(anioSalidaTTS, mesSalidaTTS, diaSalidaTTS, horaSalidaTTS, minutosSalidaTTS, Convert.ToInt32(0));

                    Int32[] fechaEntradaTTS = parseFechas(lineTTS.First(), "fecha Entrada", '/', agentIdSTS, "TTS"); //agentIdSTS es igual a agentIdTTS
                    Int32 diaEntradaTTS = Convert.ToInt32(fechaEntradaTTS[0]);
                    Int32 mesEntradaTTS = Convert.ToInt32(fechaEntradaTTS[1]);
                    Int32 anioEntradaTTS = Convert.ToInt32(fechaEntradaTTS[2]);
                    Int32[] horarioEntradaTTS = parseHorarios(lineTTS.First(), "Horario Entrada", ':', agentIdSTS, "TTS"); //agentIdSTS es igual a agentIdTTS
                    Int32 horaEntradaTTS = Convert.ToInt32(horarioEntradaTTS[0]);
                    Int32 minutosEntradaTTS = Convert.ToInt32(horarioEntradaTTS[1]);

                    DateTime dateTimeEntradaTTS = new DateTime(anioEntradaTTS, mesEntradaTTS, diaEntradaTTS, horaEntradaTTS, minutosEntradaTTS, Convert.ToInt32(0));

                    var metricValue = ScheduleAdherenceMetric.CalculateMetricValue(dateTimeEntradaSTS, dateTimeEntradaTTS, dateTimeSalidaSTS, dateTimeSalidaTTS);

                    this.calculatedValues.Add(agentIdSTS, metricValue);

                }
                else
                {
                   //Si no se encontro el legajo en el TTS, no importa para esta metrica
                }
            }            
        }

        /*
         * Retorna Int32[] con dia, mes, anio en este orden
         * 
         */
        public static Int32[] parseFechas(Dictionary<string,string> line, string nombreCampo, char separador, int agentId, string tipoArchivo)
        {
            Int32[] result = new Int32[3];

            string[] fechaStr = null;
            fechaStr = line[nombreCampo].Split(separador);

            if (fechaStr.Length != 3)
            {
                throw new System.ArgumentException("Formato " + nombreCampo + " para agente " + agentId + " distinto a dd/MM/yyyy en archivo " + tipoArchivo + ".");
            }
                        
            result[0] = Convert.ToInt32(fechaStr[0]); //dia
            result[1] = Convert.ToInt32(fechaStr[1]); //mes
            result[2] = Convert.ToInt32(fechaStr[2]); //anio
            
            return result;
        }

        /*
         * Retorna Int32[] con hora, minutos en este orden
         * 
         */
        public static Int32[] parseHorarios(Dictionary<string, string> line, string nombreCampo, char separador, int agentId, string tipoArchivo)
        {
            Int32[] result = new Int32[3];

            string[] fechaStr = null;
            fechaStr = line[nombreCampo].Split(separador);

            if (fechaStr.Length != 2)
            {
                throw new System.ArgumentException("Formato " + nombreCampo + " para agente " + agentId + " distinto a HH:mm en archivo " + tipoArchivo + ".");
            }

            result[0] = Convert.ToInt32(fechaStr[0]); //hora
            result[1] = Convert.ToInt32(fechaStr[1]); //minutos

            return result;
        }

    
    }
}
