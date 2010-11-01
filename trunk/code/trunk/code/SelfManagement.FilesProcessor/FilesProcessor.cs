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
                    if ( (Path.GetFileName(processedFile.FileSystemPath).Substring(0,2) == "HF") || Path.GetFileName(processedFile.FileSystemPath).Substring(0,3) == "STS" )
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
                        this.metricsRepository.CreateProcessedFile(new ProcessedFile
                        {
                            FileSystemPath = processedFile.FilePath,
                            FileType = (int)processedFile.ExternalSystemFile,
                            DateProcessed = DateTime.Now,
                            DateLastModified = File.GetLastWriteTime(processedFile.FilePath),
                            Log = "",
                            HasErrors = false
                        });
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
                                var agentCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(legajo);
                                var agentSupervisorId = this.metricsRepository.RetrieveAgentSupervisorId(legajo);
                                var supervisorCampaingId = this.metricsRepository.RetrieveUserActualCampaingId(agentSupervisorId);
                                if (agentCampaingId != supervisorCampaingId)
                                {
                                    throw new ApplicationException("Data Inconsistency");
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
                            // To bypass the metrics throw generated by the HF file taken from 'nuestros' dir
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

            if (hfFile.Count == 1)
            {
                var dataLines = filesToProcess[0].DataLines;

                foreach (var line in dataLines)
                {
                    var legajo = Convert.ToInt32(line["legajo"]);
                    var dni = Convert.ToInt32(line["dni"]);
                    var nombre = line["nombre"];
                    var apellido = line["apellido"];
                    var sueldo = Convert.ToDecimal(line["sueldo bruto"]);
                    var jornada = line["Tipo Jornada"];
                    var status = line[" Status"];
                    var idSupervisor = Convert.ToInt32(line["idSupervisor"]);
                    var strFecha = line[" fecha ingreso"].Split('/');
                    var fechaIngreso = new DateTime(Convert.ToInt32(strFecha[2]), //Año
                                                    Convert.ToInt32(strFecha[1]), //Mes
                                                    Convert.ToInt32(strFecha[0])); //Dia
                    var idCampaña = Convert.ToInt32(line[" idCampania"]);

                    Console.WriteLine("Grabando Agente: " + legajo);
                    if (!this.membershipService.ExistsUser(nombre + "." + apellido))
                    {
                        this.membershipService.CreateUser(legajo, nombre + "." + apellido, dni.ToString(), nombre + "." + apellido + "@selfmanagement.com");
                        this.membershipService.CreateProfile(nombre + "." + apellido, dni.ToString(), nombre, apellido, sueldo, jornada, status, fechaIngreso);
                        this.membershipService.AddUserToRol(nombre + "." + apellido, SelfManagementRoles.Agent);
                        this.metricsRepository.CreateSupervisorAgent(new SupervisorAgent { AgentId = legajo, SupervisorId = idSupervisor });

                        var supervisores = this.campaingRepository.RetrieveCampaingSupervisors(idCampaña);

                        if (supervisores.Count > 0)
                        {
                            var supervisor = (from s in supervisores
                                              where s.InnerUserId == idSupervisor
                                              select s).ToList();

                            if (supervisor.Count > 0)
                            {
                                var agent = this.membershipService.RetrieveAgent(legajo);
                                this.campaingRepository.AddAgent(idCampaña, agent);
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

                }
            }
        }

        private void ProcessTTSFilesForExtraHours(IList<IDataFile> files)
        {
            var filesToProcess = files;

            var ttsFiles = (from f in filesToProcess
                            where f.ExternalSystemFile == ExternalSystemFiles.TTS //&& f.FileDate.Month == DateTime.Now.Month && f.FileDate.Year == DateTime.Now.Year
                            select f).ToList<IDataFile>();

            if (ttsFiles.Count > 0)
            {
                foreach (var file in ttsFiles)
                {
                    var dataLines = file.DataLines;
                    var fileMonth = Convert.ToByte(file.FileDate.Month);
                    var fileYear = Convert.ToInt16(file.FileDate.Year);

                    foreach (var line in dataLines)
                    {
                        var agentId = Convert.ToInt32(line["legajo"]);

                        Console.WriteLine("Procesando TTS para Agente: " + agentId + " - Fecha: " + file.FileDate.ToString("dd/MM/yyyy"));

                        var strFecha = line["fecha Entrada"].Split('/');
                        var strHora = line["Horario Entrada"].Split(':');
                        var entranceDate = new DateTime(Convert.ToInt32(strFecha[2]) /*Año*/, Convert.ToInt32(strFecha[1]) /*Mes*/, Convert.ToInt32(strFecha[0]) /*Dia*/,
                                                        Convert.ToInt32(strHora[0]) /*Horas*/, Convert.ToInt32(strHora[1]) /*Minutos*/, 0 /*Segundos*/);

                        strFecha = line["fecha Salida"].Split('/');
                        strHora = line["Horario Salida"].Split(':');
                        var exitDate = new DateTime(Convert.ToInt32(strFecha[2]), Convert.ToInt32(strFecha[1]), Convert.ToInt32(strFecha[0]),
                                                       Convert.ToInt32(strHora[0]), Convert.ToInt32(strHora[1]), 0);

                        if (exitDate < entranceDate)
                        {
                            //TODO: Loguear error de formato
                        }
                        else
                        {
                            var agent = this.membershipService.RetrieveAgent(agentId);

                            if (agent == null)
                            {
                                //TODO: Loguear error de que el agente indicado no existe
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

                                    var workdayHours = (agent.Workday == "FTE") ? 8.0 : 6.0;
                                    var actualWorkedHours = exitDate.Subtract(entranceDate).TotalHours;

                                    if (actualWorkedHours > workdayHours)
                                    {
                                        var extraHours = actualWorkedHours - workdayHours;

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
                                    agentSchedule.LastDayModified = Convert.ToByte(file.FileDate.Day);
                                }

                                this.metricsRepository.SaveOrUpdateMonthlySchedule(agentSchedule);
                            }
                        }
                    }
                }
            }
        }
    }
}
