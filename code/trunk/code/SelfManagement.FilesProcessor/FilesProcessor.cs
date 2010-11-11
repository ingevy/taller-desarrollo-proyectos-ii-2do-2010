namespace CallCenter.SelfManagement.FilesProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Metric;
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

            this.ProcessHumanForceFile(filesToProcess);

            this.ProcessTTSFilesForExtraHours(filesToProcess);

            this.ProcessMetrics(filesToProcess);
        }

        private void ProcessMetrics(IList<IDataFile> files)
        {
            Console.WriteLine("Procesando Metricas...");

            var filesToProcess = files;

            if (filesToProcess.Count > 0)
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
                        var metricTypes = metric.CLRType.Split(',');
                        IMetric metricProcessor = (IMetric)Activator.CreateInstance(metricTypes[1], metricTypes[0]).Unwrap();

                        try
                        {
                            metricProcessor.ProcessFiles(groupedFiles[date]); // TODO: Capture error event in metrics and log file error into DB
                            var metricValues = metricProcessor.CalculatedValues;

                            Console.WriteLine("Grabando Metrica: " + metric.MetricName + " - Fecha: " + metricProcessor.MetricDate.ToString("dd/MM/yyyy"));
                            foreach (var legajo in metricValues.Keys)
                            {
                                var agentCampaingId = this.metricsRepository.RetrieveUserCampaingId(legajo, metricProcessor.MetricDate);
                                var agentSupervisorId = this.metricsRepository.RetrieveAgentSupervisorId(legajo);
                                var supervisorCampaingId = this.metricsRepository.RetrieveUserCampaingId(agentSupervisorId, metricProcessor.MetricDate);
                                if (agentCampaingId != supervisorCampaingId)
                                {
                                    throw new ApplicationException("Inconsistencia: Campaña Agente = "+Convert.ToInt32(agentCampaingId)+" - Campaña Supervisor = "+Convert.ToInt32(agentSupervisorId));
                                }

                                var campaingMetrics = this.campaingRepository.RetrieveCampaingMetricLevels(agentCampaingId);

                                if ( (from cm in campaingMetrics where cm.MetricId == metric.Id select cm).ToList().Count > 0 )
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
                                    this.metricsRepository.CreateOrUpdateSupervisorMetric(agentSupervisorId, agentCampaingId, metric.Id, metricProcessor.MetricDate, metricValues[legajo]);
                                    this.metricsRepository.CreateOrUpdateCampaingMetric(agentCampaingId, metric.Id, metricProcessor.MetricDate, metricValues[legajo]);
                                }
                            }
                        }
                        catch
                        {
                            // Para evitar el throw de las metricas por el archivo HF que esta en el directorio
                        }
                    }
                }
            }
        }

        private void ProcessHumanForceFile(IList<IDataFile> files)
        {
            Console.WriteLine("Procesando Archivo HF...");

            var filesToProcess = files;

            var hfFile = (from f in filesToProcess
                          where f.ExternalSystemFile == ExternalSystemFiles.HF
                          select f).ToList<IDataFile>();

            if (hfFile.Count == 1) //El archivo HF es siempre uno solo, que se va actualizando
            {
                this.CreateProcessedFileFromDataFile(hfFile[0]);

                var dataLines = hfFile[0].DataLines;

                foreach (var line in dataLines)
                {
                    var hfAgent = new HfAgent(line);

                    var dbAgent = this.membershipService.RetrieveAgent(hfAgent.Legajo);

                    if (dbAgent == null) //Caso 1: El agente no existe en el sistema
                    {
                        Console.WriteLine("Creando Agente: " + hfAgent.Legajo);
                        this.ProcessNewAgentFromHF(hfAgent);
                    }
                    else //Caso 2: El agente ya existe en el sistema. Actualizar datos
                    {
                        Console.WriteLine("Modificando Agente: " + hfAgent.Legajo);
                        this.ProcessUpdatedAgentFromHF(hfAgent, dbAgent);
                    }
                }
            }
        }

        private void ProcessTTSFilesForExtraHours(IList<IDataFile> files)
        {
            var filesToProcess = files;

            var timeFiles = (from f in filesToProcess
                            where f.ExternalSystemFile == ExternalSystemFiles.TTS || f.ExternalSystemFile == ExternalSystemFiles.STS //&& f.FileDate.Month == DateTime.Now.Month && f.FileDate.Year == DateTime.Now.Year
                            select f).ToList<IDataFile>();

            if (timeFiles.Count > 0)
            {
                var groupedFiles = this.GroupFilesToProcessByDate(timeFiles);

                foreach (var date in groupedFiles.Keys)
                {
                    if (groupedFiles[date].Count == 2)
                    {
                        var ttsFile = (from f in groupedFiles[date]
                                       where f.ExternalSystemFile == ExternalSystemFiles.TTS
                                       select f).ToList().FirstOrDefault();
                        var stsFile = (from f in groupedFiles[date]
                                       where f.ExternalSystemFile == ExternalSystemFiles.STS
                                       select f).ToList().FirstOrDefault();

                        if ((ttsFile == null) || (stsFile == null))
                        {
                            throw new ArgumentException("No se pudo encontrar el STS o el TTS para la fecha "+date.ToString("dd-MM-yyyy"));
                        }

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

                            var strFecha = line["fecha Entrada"].Split('/');
                            var strHora = line["Horario Entrada"].Split(':');
                            var entranceDate = new DateTime(Convert.ToInt32(strFecha[2]) /*Año*/, Convert.ToInt32(strFecha[1]) /*Mes*/, Convert.ToInt32(strFecha[0]) /*Dia*/,
                                                            Convert.ToInt32(strHora[0]) /*Horas*/, Convert.ToInt32(strHora[1]) /*Minutos*/, 0 /*Segundos*/);

                            strFecha = line["fecha Salida"].Split('/');
                            strHora = line["Horario Salida"].Split(':');
                            var exitDate = new DateTime(Convert.ToInt32(strFecha[2]), Convert.ToInt32(strFecha[1]), Convert.ToInt32(strFecha[0]),
                                                            Convert.ToInt32(strHora[0]), Convert.ToInt32(strHora[1]), 0);

                            var agent = this.membershipService.RetrieveAgent(agentId);

                            if (agent == null)
                            {
                                this.metricsRepository.LogInProcessedFile(ttsFile.FilePath, "Linea " + (dataLines.IndexOf(line)+1) + ": El agente " + line["legajo"] + " no se encuentra en la BD");
                            }
                            else
                            {
                                var agentSchedule = this.metricsRepository.RetrieveAgentMonthlySchedule(agentId, fileYear, fileMonth);

                                if (agentSchedule == null)
                                {
                                    agentSchedule = new MonthlySchedule
                                    {
                                        InnerUserId = agentId,
                                        Year = fileYear,
                                        Month = fileMonth,
                                        ExtraHoursWorked50 = 0,
                                        ExtraHoursWorked100 = 0,
                                        TotalHoursWorked = 0,
                                        GrossSalary = Convert.ToDecimal(agent.GrossSalary),
                                        LastDayModified = 0
                                    };
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

                    }
                }
            }
        }

        private void ProcessNewAgentFromHF(HfAgent hfAgent)
        {
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
                    //TODO: Loguear que el supervisor indicado no existe o no esta asignado a la campaña indicada
                }
            }
            else
            {
                //TODO: Loguear que la capaña indicada no existe o no tiene supervisores asignados
            }
        }

        private void ProcessUpdatedAgentFromHF(HfAgent hfAgent, Agent dbAgent)
        {
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
                    //TODO: Loguear inconsistencia de datos
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
                    //TODO: Loguear inconsistencia de datos
                }
            }
        }

        private double GetScheduledHours(IList<Dictionary<string,string>> stsLines, int agentId)
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
                var strFecha = agentLine["fecha Entrada"].Split('/');
                var strHora = agentLine["Horario Entrada"].Split(':');
                var entranceDate = new DateTime(Convert.ToInt32(strFecha[2]) /*Año*/, Convert.ToInt32(strFecha[1]) /*Mes*/, Convert.ToInt32(strFecha[0]) /*Dia*/,
                                                Convert.ToInt32(strHora[0]) /*Horas*/, Convert.ToInt32(strHora[1]) /*Minutos*/, 0 /*Segundos*/);

                strFecha = agentLine["fecha Salida"].Split('/');
                strHora = agentLine["Horario Salida"].Split(':');
                var exitDate = new DateTime(Convert.ToInt32(strFecha[2]), Convert.ToInt32(strFecha[1]), Convert.ToInt32(strFecha[0]),
                                                Convert.ToInt32(strHora[0]), Convert.ToInt32(strHora[1]), 0);

                scheduledHours = exitDate.Subtract(entranceDate).TotalHours;
            }

            return scheduledHours;
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
        }
    }
}
