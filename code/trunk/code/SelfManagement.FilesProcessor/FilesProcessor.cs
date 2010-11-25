namespace CallCenter.SelfManagement.FilesProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Metric;
    using CallCenter.SelfManagement.Metric.Helpers;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using System.Globalization;
    using CallCenter.SelfManagement.FilesProcessor.Helpers;

    public class FilesProcessor
    {
        private readonly IMetricsRepository metricsRepository;
        private readonly IMembershipService membershipService;
        private readonly ICampaingRepository campaingRepository;

        public FilesProcessor(IMetricsRepository metricsRepository, IMembershipService membershipService, ICampaingRepository campaingRepository)
        {
            this.metricsRepository = metricsRepository;
            this.membershipService = membershipService;
            this.campaingRepository = campaingRepository;
        }

        private IList<IDataFile> GetFilesToProcess()
        {
            var filesToProcess = new List<IDataFile>();
            var filter = "*.csv";
            var paths = Directory.GetFiles(ConfigurationManager.AppSettings["ExternalFilesLocation"].ToString(), filter);

            foreach (var path in paths)
            {
                var processedFile = this.metricsRepository.RetrieveProcessedFileByPath(path);

                if (processedFile == null)
                {
                    filesToProcess.Add(new DataFile(path));
                }
                else
                {
                    if (Path.GetFileName(processedFile.FileSystemPath).Substring(0,2) == "HF")
                    {
                        if (File.GetLastWriteTime(path) != processedFile.DateLastModified)
                        {
                            filesToProcess.Add(new DataFile(path));
                        }
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
            }

            return filesToProcess;
        }

        private Dictionary<DateTime,IList<IDataFile>> GroupFilesToProcessByDate(IList<IDataFile> filesToProcess)
        {
            var groupedFiles = new Dictionary<DateTime, IList<IDataFile>>();

            foreach (var file in filesToProcess)
            {
                groupedFiles[file.FileDate] = new List<IDataFile>();
            }

            foreach (var file in filesToProcess)
            {
                groupedFiles[file.FileDate].Add(file);
            }

            return groupedFiles;
        }

        public void ProcessFiles()
        {
            var filesToProcess = this.GetFilesToProcess();

            Console.WriteLine("Procesando Archivo HF...");
            this.ProcessHumanForceFile(filesToProcess);
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("Procesando Horas Extra...");
            this.ProcessTTSFilesForExtraHours(filesToProcess);
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("Procesando Metricas...");
            this.ProcessMetrics(filesToProcess);
            Console.WriteLine(Environment.NewLine);
        }

        private void ProcessMetrics(IList<IDataFile> files)
        {
            var filesToProcess = (from f in files
                                  where f.ExternalSystemFile != ExternalSystemFiles.HF
                                  select f).ToList();

            if (filesToProcess.Count > 0)
            {
                try
                {
                    var groupedFiles = this.GroupFilesToProcessByDate(filesToProcess);

                    foreach (var date in groupedFiles.Keys)
                    {
                        foreach (var processedFile in groupedFiles[date])
                        {
                            this.CreateProcessedFileFromDataFile(processedFile);
                        }

                        var avMetrics = this.metricsRepository.RetrieveAvailableMetrics();

                        foreach (var metric in avMetrics)
                        {
                            try
                            {
                                var metricTypes = metric.CLRType.Split(',');
                                IMetric metricProcessor = (IMetric)Activator.CreateInstance(metricTypes[1], metricTypes[0]).Unwrap();

                                try
                                {
                                    metricProcessor.ProcessFiles(groupedFiles[date]);
                                    var metricValues = metricProcessor.CalculatedValues;

                                    Console.WriteLine("Grabando Metrica: " + metric.MetricName + " - Fecha: " + metricProcessor.MetricDate.ToString("dd/MM/yyyy"));
                                    foreach (var legajo in metricValues.Keys)
                                    {
                                        if (!this.campaingRepository.ExistsAgent(legajo))
                                        {
                                            throw new MetricException("El agente " + legajo + " no existe en la base de datos");
                                        }

                                        var agentCampaingId = this.metricsRepository.RetrieveUserCampaingId(legajo, metricProcessor.MetricDate);
                                        var agentSupervisorId = this.metricsRepository.RetrieveAgentSupervisorId(legajo);
                                        var supervisorCampaingId = this.metricsRepository.RetrieveUserCampaingId(agentSupervisorId, metricProcessor.MetricDate);
                                        if (agentCampaingId != supervisorCampaingId)
                                        {
                                            throw new MetricException("Inconsistencia: Campaña Agente = " + Convert.ToInt32(agentCampaingId) + " - Campaña Supervisor = " + Convert.ToInt32(agentSupervisorId));
                                        }

                                        var campaingMetrics = this.campaingRepository.RetrieveCampaingMetricLevels(agentCampaingId);

                                        if ((from cm in campaingMetrics where cm.MetricId == metric.Id select cm).ToList().Count > 0)
                                        {
                                            var userMetric = new UserMetric
                                            {
                                                CampaingId = agentCampaingId,
                                                InnerUserId = legajo,
                                                MetricId = metric.Id,
                                                Date = metricProcessor.MetricDate,
                                                Value = metricValues[legajo]
                                            };
                                            this.metricsRepository.CreateAgentMetric(userMetric);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    var metricFiles = (from f in groupedFiles[date]
                                                       from f2 in metricProcessor.ExternalFilesNeeded
                                                       where f.ExternalSystemFile == f2
                                                       select f).ToList<IDataFile>();

                                    foreach (var neededFile in metricFiles)
                                    {
                                        this.metricsRepository.LogInProcessedFile(neededFile.FilePath, e.Message+Environment.NewLine);
                                    }

                                    throw;
                                }

                                this.metricsRepository.CreateOrUpdateSupervisorMetric(metric.Id, metricProcessor.MetricDate);
                                this.metricsRepository.CreateOrUpdateCampaingMetric(metric.Id, metricProcessor.MetricDate);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error procesando metrica " + metric.MetricName + " para la fecha " + date.ToString("dd-MM-yyyy") + ": " + e.Message);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error general durante el procesamiento de metricas: " + e.Message);
                }
            }
        }

        private void ProcessHumanForceFile(IList<IDataFile> files)
        {
            var hfFile = (from f in files
                          where f.ExternalSystemFile == ExternalSystemFiles.HF
                          select f).ToList<IDataFile>();

            if (hfFile.Count == 1) //El archivo HF es siempre uno solo, que se va actualizando
            {
                try
                {
                    this.CreateProcessedFileFromDataFile(hfFile[0]);

                    var dataLines = hfFile[0].DataLines;

                    foreach (var line in dataLines)
                    {
                        try
                        {
                            var hfAgent = new HfAgent(line);

                            var dbAgent = this.membershipService.RetrieveAgent(hfAgent.Legajo);

                            if (dbAgent == null) //Caso 1: El agente no existe en el sistema
                            {
                                Console.WriteLine("Creando Agente: " + hfAgent.Legajo);
                                try
                                {
                                    this.ProcessNewAgentFromHF(hfAgent);
                                }
                                catch (Exception e)
                                {
                                    this.metricsRepository.LogInProcessedFile(hfFile[0].FilePath, "Linea " + (dataLines.IndexOf(line) + 1) + ": " + e.Message + Environment.NewLine);
                                    Console.WriteLine("     " + e.Message);
                                }
                            }
                            else //Caso 2: El agente ya existe en el sistema. Actualizar datos
                            {
                                Console.WriteLine("Modificando Agente: " + hfAgent.Legajo);
                                try
                                {
                                    this.ProcessUpdatedAgentFromHF(hfAgent, dbAgent);
                                }
                                catch (Exception e)
                                {
                                    this.metricsRepository.LogInProcessedFile(hfFile[0].FilePath, "Linea " + (dataLines.IndexOf(line) + 1) + ": " + e.Message + Environment.NewLine);
                                    Console.WriteLine("     " + e.Message);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            this.metricsRepository.LogInProcessedFile(hfFile[0].FilePath, "Linea " + (dataLines.IndexOf(line) + 1) + ": " + e.Message + Environment.NewLine);
                            Console.WriteLine("     " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error procesando archivo HF: "+e.Message);
                }
            }
            else if (hfFile.Count > 1)
            {
                Console.WriteLine("Se encontro mas de un archivo HF. Se detiene el proceso");
            }
        }

        private void ProcessTTSFilesForExtraHours(IList<IDataFile> files)
        {
            var timeFiles = (from f in files
                            where f.ExternalSystemFile == ExternalSystemFiles.TTS || f.ExternalSystemFile == ExternalSystemFiles.STS
                            select f).ToList<IDataFile>();

            if (timeFiles.Count > 0)
            {
                var groupedFiles = this.GroupFilesToProcessByDate(timeFiles);

                foreach (var date in groupedFiles.Keys)
                {
                    if (groupedFiles[date].Count == 2) //Por cada fecha debo tener 2 archivos (par TTS-STS)
                    {
                        var ttsFile = (from f in groupedFiles[date]
                                       where f.ExternalSystemFile == ExternalSystemFiles.TTS
                                       select f).ToList().FirstOrDefault();
                        var stsFile = (from f in groupedFiles[date]
                                       where f.ExternalSystemFile == ExternalSystemFiles.STS
                                       select f).ToList().FirstOrDefault();

                        if ((ttsFile == null) || (stsFile == null))
                        {
                            Console.WriteLine("No se pudo encontrar el STS o el TTS para la fecha " + date.ToString("dd-MM-yyyy"));
                        }
                        else
                        {
                            try
                            {
                                this.CreateProcessedFileFromDataFile(ttsFile);
                                this.CreateProcessedFileFromDataFile(stsFile);

                                var dataLines = ttsFile.DataLines;
                                var stsLines = stsFile.DataLines;
                                var fileMonth = Convert.ToByte(ttsFile.FileDate.Month);
                                var fileYear = Convert.ToInt16(ttsFile.FileDate.Year);

                                foreach (var line in dataLines)
                                {
                                    var agentId = Convert.ToInt32(line["legajo"]);

                                    Console.WriteLine("Procesando Horas para Agente: " + agentId + " - Fecha: " + ttsFile.FileDate.ToString("dd/MM/yyyy"));

                                    try
                                    {
                                        var entranceDate = this.ParseDateHourFromFileFields(line["fecha Entrada"], line["Horario Entrada"]);
                                        var exitDate = this.ParseDateHourFromFileFields(line["fecha Salida"], line["Horario Salida"]);

                                        var agent = this.membershipService.RetrieveAgent(agentId);

                                        if (agent == null)
                                        {
                                            throw new ArgumentException("El agente " + agentId + " no existe en la base de datos");                                        
                                        }
                                        else
                                        {
                                            var agentSchedule = this.metricsRepository.RetrieveAgentMonthlySchedule(agentId, fileYear, fileMonth);

                                            if (agentSchedule == null)
                                            {
                                                agentSchedule = new MonthlySchedule { InnerUserId = agentId, Year = fileYear, Month = fileMonth,
                                                                                      ExtraHoursWorked50 = 0, ExtraHoursWorked100 = 0, TotalHoursWorked = 0, 
                                                                                      GrossSalary = Convert.ToDecimal(agent.GrossSalary), LastDayModified = 0 };
                                            }
                                            if (entranceDate.Day > agentSchedule.LastDayModified)
                                            {
                                                var workdayHours = this.GetScheduledHours(stsLines, agent.InnerUserId);
                                                var actualWorkedHours = exitDate.Subtract(entranceDate).TotalHours;

                                                if (actualWorkedHours > workdayHours)
                                                {
                                                    var extraHours = (workdayHours == 0.0) ? actualWorkedHours : actualWorkedHours - workdayHours;

                                                    if ((entranceDate.DayOfWeek.Equals(DayOfWeek.Saturday)) ||
                                                            (entranceDate.DayOfWeek.Equals(DayOfWeek.Sunday)) ||
                                                            (this.metricsRepository.IsHolidayDate(entranceDate)))
                                                    {
                                                        agentSchedule.ExtraHoursWorked100 += Convert.ToInt32(Math.Round(extraHours));
                                                    }
                                                    else
                                                    {
                                                        agentSchedule.ExtraHoursWorked50 += Convert.ToInt32(Math.Round(extraHours));
                                                    }
                                                }

                                                agentSchedule.TotalHoursWorked += Convert.ToInt32(Math.Round(actualWorkedHours));
                                                agentSchedule.LastDayModified = Convert.ToByte(ttsFile.FileDate.Day);
                                            }

                                            this.metricsRepository.SaveOrUpdateMonthlySchedule(agentSchedule);
                                        }
                                    }
                                    catch (STSException e)
                                    {
                                        this.metricsRepository.LogInProcessedFile(ttsFile.FilePath, "Linea " + (dataLines.IndexOf(line) + 1) + ": Error STS #" + e.Message + Environment.NewLine);
                                        this.metricsRepository.LogInProcessedFile(stsFile.FilePath, e.Message + Environment.NewLine);
                                        Console.WriteLine("     " + e.Message);
                                    }
                                    catch (Exception e)
                                    {
                                        this.metricsRepository.LogInProcessedFile(ttsFile.FilePath, "Linea " + (dataLines.IndexOf(line) + 1) + ": " + e.Message + Environment.NewLine);
                                        this.metricsRepository.LogInProcessedFile(stsFile.FilePath, "Error TTS-Linea " + (dataLines.IndexOf(line) + 1) + ": " + e.Message + Environment.NewLine);
                                        Console.WriteLine("     " + e.Message);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error procesando archivos TTS-STS para la fecha " + date.ToString("dd-MM-yyyy") + ": " + e.Message);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se pudo encontrar el STS o el TTS para la fecha " + date.ToString("dd-MM-yyyy"));
                    }
                }
            }
        }

        private void ProcessNewAgentFromHF(HfAgent hfAgent)
        {
            var msg = "";
            if (!this.campaingRepository.ExistsSupervisor(hfAgent.SupervisorId))
            {
                msg += "El supervisor " + hfAgent.SupervisorId + " no existe en la base de datos.";
            }
            if (!this.campaingRepository.ExistsCampaing(hfAgent.CampaingId))
            {
                msg += "La campaña " + hfAgent.CampaingId + " no existe en la base de datos.";
            }
            if (msg != "")
            {
                throw new ArgumentException(msg);
            }

            var username = hfAgent.Name + "." + hfAgent.LastName;
            if (this.membershipService.ExistsUser(username)) //Username ya tomado por otro agente
            {
                var i = 2;
                var setted = false;
                while (!setted)
                {
                    username = username + "." + i;
                    if (!this.membershipService.ExistsUser(username))
                    {
                        setted = true;
                    }
                    i++;
                }
            }

            this.membershipService.CreateUser(hfAgent.Legajo, username, hfAgent.DNI, username + "@selfmanagement.com");
            this.membershipService.CreateProfile(username, hfAgent.DNI, hfAgent.Name, hfAgent.LastName, hfAgent.Salary, hfAgent.Workday, hfAgent.Status, hfAgent.IncorporationDate);
            this.membershipService.AddUserToRol(username, SelfManagementRoles.Agent);
            this.metricsRepository.CreateSupervisorAgent(new SupervisorAgent { AgentId = hfAgent.Legajo, SupervisorId = hfAgent.SupervisorId });

            var supervisores = this.campaingRepository.RetrieveCampaingSupervisors(hfAgent.CampaingId);

            if (supervisores.Count > 0)
            {
                var supervisor = (from s in supervisores
                                  where s.InnerUserId == hfAgent.SupervisorId
                                  select s).ToList();

                if (supervisor.Count > 0)
                {
                    var agent = this.membershipService.RetrieveAgent(hfAgent.Legajo);
                    this.campaingRepository.AddAgent(hfAgent.CampaingId, agent);
                }
                else
                {
                    throw new ArgumentException("El supervisor "+hfAgent.SupervisorId+" no esta asignado a la campaña "+hfAgent.CampaingId);
                }
            }
            else
            {
                throw new ArgumentException("La campaña "+hfAgent.CampaingId+" no tiene supervisores asignados");
            }
        }

        private void ProcessUpdatedAgentFromHF(HfAgent hfAgent, Agent dbAgent)
        {
            var msg = "";
            if (!this.campaingRepository.ExistsSupervisor(hfAgent.SupervisorId))
            {
                msg += "El supervisor " + hfAgent.SupervisorId + " no existe en la base de datos.";
            }
            if (!this.campaingRepository.ExistsCampaing(hfAgent.CampaingId))
            {
                msg += "La campaña " + hfAgent.CampaingId + " no existe en la base de datos.";
            }
            if (msg != "")
            {
                throw new ArgumentException(msg);
            }

            //TODO: Verificar y actualizar propiedades del perfil (salary, jornada, status)

            var agentActualCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(dbAgent.InnerUserId);

            if ((hfAgent.SupervisorId != dbAgent.SupervisorId) && (hfAgent.CampaingId == agentActualCampaingId)) //Cambio de supervisor manteniendo campaña
            {
                var supervisorActualCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(hfAgent.SupervisorId);

                //La campaña actual del nuevo supervisor debe ser la misma que tenia el agente
                if (supervisorActualCampaingId == agentActualCampaingId)
                {
                    this.metricsRepository.ChangeAgentSupervisor(dbAgent.InnerUserId, hfAgent.SupervisorId);
                }
                else
                {
                    throw new ArgumentException("El supervisor " + hfAgent.SupervisorId + " no esta asignado a la misma campaña que el agente " + dbAgent.InnerUserId);
                }
            }
            else if ((hfAgent.SupervisorId != dbAgent.SupervisorId) && (hfAgent.CampaingId != agentActualCampaingId)) //Cambio de supervisor y de campaña
            {
                var supervisorActualCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(hfAgent.SupervisorId);

                //La campaña actual del nuevo supervisor debe ser la misma que se informa en el archivo HF
                if (supervisorActualCampaingId == hfAgent.CampaingId)
                {
                    this.metricsRepository.ChangeAgentSupervisorAndCampaing(dbAgent.InnerUserId, hfAgent.SupervisorId, hfAgent.CampaingId);
                }
                else
                {
                    throw new ArgumentException("El supervisor " + hfAgent.SupervisorId + " no esta asignado a la campaña " + hfAgent.CampaingId);
                }
            }
        }

        private double GetScheduledHours(IList<Dictionary<string,string>> stsLines, int agentId)
        {
            try
            {
                var scheduledHours = 0.0;

                var agentLine = (from l in stsLines
                                 where Convert.ToInt32(l["legajo"]) == agentId
                                 select l).ToList().FirstOrDefault();

                if (agentLine == null)
                {
                    scheduledHours = 0.0;
                }
                else
                {
                    var entranceDate = this.ParseDateHourFromFileFields(agentLine["fecha Entrada"], agentLine["Horario Entrada"]);
                    var exitDate = this.ParseDateHourFromFileFields(agentLine["fecha Salida"], agentLine["Horario Salida"]);

                    scheduledHours = exitDate.Subtract(entranceDate).TotalHours;
                }

                return scheduledHours;
            }
            catch (Exception e)
            {
                throw new STSException(e.Message);
            }
        }

        private void CreateProcessedFileFromDataFile(IDataFile dataFile)
        {
            var originalProcessedFile = this.metricsRepository.RetrieveProcessedFileByPath(dataFile.FilePath);

            if (originalProcessedFile == null)
            {
                var processedFile = new ProcessedFile
                {
                    FileSystemPath = dataFile.FilePath,
                    FileType = (int)dataFile.ExternalSystemFile,
                    DateProcessed = DateTime.Now,
                    DateLastModified = File.GetLastWriteTime(dataFile.FilePath),
                    Log = "",
                    HasErrors = false
                };

                this.metricsRepository.CreateProcessedFile(processedFile);
            }
            else
            {
                this.metricsRepository.CleanProcessedFile(originalProcessedFile);
            }
        }

        private DateTime ParseDateHourFromFileFields(string fecha, string hora)
        {
            var strFecha = fecha.Split('/');
            var strHora = hora.Split(':');
            var date = new DateTime(Convert.ToInt32(strFecha[2]) /*Año*/, Convert.ToInt32(strFecha[1]) /*Mes*/, Convert.ToInt32(strFecha[0]) /*Dia*/,
                                    Convert.ToInt32(strHora[0]) /*Horas*/, Convert.ToInt32(strHora[1]) /*Minutos*/, 0 /*Segundos*/);

            return date;
        }
    }
}
