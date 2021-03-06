﻿namespace CallCenter.SelfManagement.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI.DataVisualization.Charting;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;

    public class AgentController : Controller
    {
        private readonly Color FontColor = Color.FromArgb(51, 93, 76);
        private readonly Color BackColor = Color.FromArgb(221, 221, 238);

        private readonly ICampaingRepository campaingRepository;
        private readonly IMetricsRepository metricsRepository;
        private readonly IMembershipService membershipService;

        public AgentController()
            : this(new RepositoryFactory().GetCampaingRepository(), new RepositoryFactory().GetMetricsRepository(), new RepositoryFactory().GetMembershipService())
        {
        }

        public AgentController(ICampaingRepository campaingRepository, IMetricsRepository metricsRepository, IMembershipService membershipService)
        {
            this.campaingRepository = campaingRepository;
            this.metricsRepository = metricsRepository;
            this.membershipService = membershipService;
        }

        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Index(int? pageNumber, int? campaingId, int? supervisorId)
        {
            bool shoulPaginate;
            bool shouldIncludeCampaing;
            bool shouldIncludeSupervisor;
            int totalCount;
            int page;
            var agent = this.GetAgent(pageNumber, campaingId, supervisorId, out shoulPaginate, out shouldIncludeCampaing, out shouldIncludeSupervisor, out page, out totalCount);

            if (agent == null)
            {
                return this.View("NotFound", new AgentDetailsViewModel());
            }

            var currentSupervisor = agent.SupervisorId.HasValue ? this.membershipService.RetrieveSupervisor(agent.SupervisorId.Value) : (Supervisor)null;
            var userCampaing = campaingId.HasValue ? this.campaingRepository.RetrieveCampaingById(campaingId.GetValueOrDefault(0)) : this.campaingRepository.RetrieveCurrentCampaingByUserId(agent.InnerUserId);
            var userCampaings = this.campaingRepository.RetrieveCampaingsByUserId(agent.InnerUserId);

            var metricsDate = DateTime.Now;
            IList<string> availableMetricMonths = null;
            if (userCampaing == null)
            {
                if ((userCampaings != null) && (userCampaings.Count > 0))
                {
                    userCampaing = userCampaings.OrderByDescending(c => c.BeginDate).LastOrDefault();
                    availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);

                    var date = availableMetricMonths.LastOrDefault();

                    metricsDate = DateTime.ParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
                    metricsDate = GetEndDate(userCampaing, metricsDate.Year, metricsDate.Month);
                }
            }
            else
            {
                availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);

                if (campaingId.HasValue)
                {
                    var date = availableMetricMonths.LastOrDefault();

                    metricsDate = DateTime.ParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
                    metricsDate = GetEndDate(userCampaing, metricsDate.Year, metricsDate.Month);
                }
            }

            var model = new AgentDetailsViewModel
            {
                AgentId = agent.InnerUserId,
                AvailableSalaryMonths = this.membershipService.RetrieveAvailableMonthsByUser(agent.InnerUserId),
                AvailableMetricMonths = availableMetricMonths,
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", agent.Name, agent.LastName, agent.InnerUserId),
                CurrentSupervisor = EntityTranslator.GetSupervisorDisplayName(currentSupervisor),
                CurrentCampaingId = userCampaing != null ? userCampaing.Id : 0,
                AgentCampaings = userCampaings.Select(c => c.ToUserCampaingInfo()).ToList(),
                OptimalHourlyValue = userCampaing != null ? userCampaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture) : null,
                ObjectiveHourlyValue = userCampaing != null ? userCampaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture) : null,
                MinimumHourlyValue = userCampaing != null ? userCampaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture) : null,
                Salary = this.CalculateSalary(agent.InnerUserId, DateTime.Now),
                ShouldPaginate = shoulPaginate,
                ShouldIncludeCampaing = shouldIncludeCampaing,
                ShouldIncludeSupervisor = shouldIncludeSupervisor,
                CampaingIdForPagination = shouldIncludeCampaing ? campaingId.Value : 0,
                SupervisorIdForPagination = shouldIncludeSupervisor ? supervisorId.Value : 0,
                PageNumber = page,
                TotalPages = totalCount
            };

            if (userCampaing != null)
            {
                var endDate = GetEndDate(userCampaing, metricsDate.Year, metricsDate.Month);

                model.CurrentMetricLevel = this.CalculateMetricsLevel(agent.InnerUserId, userCampaing.Id, metricsDate, endDate, true);
                model.ProjectedMetricLevel = this.CalculateMetricsLevel(agent.InnerUserId, userCampaing.Id, metricsDate, endDate, false);
                model.CurrentCampaingMetricValues = this.CalculateCampaingMetricValues(agent.InnerUserId, userCampaing.Id, metricsDate);
            }

            return this.View(model);
        }

        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Search(string searchCriteria, int? pageNumber)
        {
            if (string.IsNullOrWhiteSpace(searchCriteria))
            {
                return this.RedirectToAction("Index", "Agent");
            }

            bool shoulPaginate;
            int totalCount;
            int page;
            searchCriteria = searchCriteria.Trim();
            var agent = this.SearchAgent(searchCriteria, pageNumber, out shoulPaginate, out page, out totalCount);

            if (agent == null)
            {
                return this.View("NotFound", new AgentDetailsViewModel { SearchCriteria = searchCriteria });
            }

            var currentSupervisor = agent.SupervisorId.HasValue ? this.membershipService.RetrieveSupervisor(agent.SupervisorId.Value) : (Supervisor)null;
            var userCampaing = this.campaingRepository.RetrieveCurrentCampaingByUserId(agent.InnerUserId);
            var userCampaings = this.campaingRepository.RetrieveCampaingsByUserId(agent.InnerUserId);

            var metricsDate = DateTime.Now;
            IList<string> availableMetricMonths = null;
            if (userCampaing == null)
            {
                if ((userCampaings != null) && (userCampaings.Count > 0))
                {
                    userCampaing = userCampaings.OrderByDescending(c => c.BeginDate).LastOrDefault();
                    availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);

                    var date = availableMetricMonths.LastOrDefault();

                    metricsDate = DateTime.ParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
                    metricsDate = GetEndDate(metricsDate.Year, metricsDate.Month);
                }
            }
            else
            {
                availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);
            }

            var model = new AgentDetailsViewModel
            {
                AgentId = agent.InnerUserId,
                SearchCriteria = searchCriteria,
                AvailableSalaryMonths = this.membershipService.RetrieveAvailableMonthsByUser(agent.InnerUserId),
                AvailableMetricMonths = availableMetricMonths,
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", agent.Name, agent.LastName, agent.InnerUserId),
                CurrentSupervisor = EntityTranslator.GetSupervisorDisplayName(currentSupervisor),
                CurrentCampaingId = userCampaing != null ? userCampaing.Id : 0,
                AgentCampaings = userCampaings.Select(c => c.ToUserCampaingInfo()).ToList(),
                OptimalHourlyValue = userCampaing != null ? userCampaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture) : null,
                ObjectiveHourlyValue = userCampaing != null ? userCampaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture) : null,
                MinimumHourlyValue = userCampaing != null ? userCampaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture) : null,
                Salary = this.CalculateSalary(agent.InnerUserId, DateTime.Now),
                ShouldPaginate = shoulPaginate,
                PageNumber = page,
                TotalPages = totalCount
            };

            if (userCampaing != null)
            {
                model.CurrentMetricLevel = this.CalculateMetricsLevel(agent.InnerUserId, userCampaing.Id, metricsDate, GetEndDate(metricsDate.Year, metricsDate.Month), true);
                model.ProjectedMetricLevel = this.CalculateMetricsLevel(agent.InnerUserId, userCampaing.Id, metricsDate, GetEndDate(metricsDate.Year, metricsDate.Month), false);
                model.CurrentCampaingMetricValues = this.CalculateCampaingMetricValues(agent.InnerUserId, userCampaing.Id, metricsDate);
            }

            return this.View(model);
        }

        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Salary(int innerUserId, string month)
        {
            var date = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
            if ((date.Year != DateTime.Now.Year) || ((date.Year == DateTime.Now.Year) && (date.Month != DateTime.Now.Month)))
            {
                date = GetEndDate(date.Year, date.Month);
            }
            else
            {
                date = DateTime.Now;
            }

            var model = this.CalculateSalary(innerUserId, date);

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { Salary = model } };
        }

        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult CampaingMetricValues(int innerUserId, int campaingId)
        {
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
            var availableMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(campaingId);
            var today = DateTime.Now.Date;
            var monthIndex = availableMonths.IndexOf(today.ToString("yyyy-MM"));
            var date = today;
            var endDate = GetEndDate(campaing, date.Year, date.Month);

            if (monthIndex == -1)
            {
                var last = availableMonths.Last().Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var lastYear = int.Parse(last[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
                var lastMonth = int.Parse(last[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
                var flag = false;

                if ((today.Year > lastYear) || ((today.Year == lastYear) && (today.Month > lastMonth)))
                {
                    flag = true;
                    monthIndex = availableMonths.Count - 1;
                }
                else
                {
                    flag = false;
                    monthIndex = 0;
                }

                var metricsDate = DateTime.ParseExact(availableMonths[monthIndex], "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;

                endDate = GetEndDate(campaing, metricsDate.Year, metricsDate.Month);
                date = flag ? endDate : metricsDate;
            }

            var model = this.CalculateCampaingMetricValues(innerUserId, campaingId, date);
            var currentMetricLevel = this.CalculateMetricsLevel(innerUserId, campaing.Id, date, endDate, true);
            var projectedMetricLevel = this.CalculateMetricsLevel(innerUserId, campaing.Id, date, endDate, false);

            return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                        {
                            CampaingMetricValues = model,
                            OptimalHourlyValue = campaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                            ObjectiveHourlyValue = campaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                            MinimumHourlyValue = campaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                            AvailableMetricMonths = availableMonths,
                            CurrentMetricMonthIndex = monthIndex,
                            CurrentMetricLevelDescription = currentMetricLevel.GetDescription(),
                            ProjectedMetricLevelDescription = projectedMetricLevel.GetDescription(),
                            CurrentMetricLevelCssClass = currentMetricLevel.GetCssClass(),
                            ProjectedMetricLevelCssClass = projectedMetricLevel.GetCssClass()
                        }
                };
        }

        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult MetricsChart(int innerUserId, int campaingId, int metricId, string month)
        {
            var campaingMetric = this.campaingRepository.RetrieveCampaingMetricLevels(campaingId).FirstOrDefault(cml => cml.MetricId == metricId);

            var yearMonth = month.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var yearNumber = int.Parse(yearMonth[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
            var monthNumber = int.Parse(yearMonth[1], NumberStyles.Integer, CultureInfo.InvariantCulture);

            var beginDate = this.GetBeginDate(campaingId, yearNumber, monthNumber).Date;
            var endDate = this.GetEndDate(campaingId, yearNumber, monthNumber).Date;

            var dates = new List<DateTime>();

            while (beginDate <= endDate)
            {
                dates.Add(beginDate.Date);
                beginDate = beginDate.AddDays(1);
            }

            var chart = new Chart()
            {
                Width = 952,
                Height = 350,
                RenderType = RenderType.BinaryStreaming,
                Palette = ChartColorPalette.BrightPastel,
                BorderlineDashStyle = ChartDashStyle.Solid,
                ToolTip = campaingMetric.Metric.MetricName,
                BorderWidth = 2,
                BorderlineColor = this.FontColor,
                BackSecondaryColor = this.BackColor,
                BackColor = this.BackColor,
                ForeColor = this.FontColor,
            };

            chart.BorderSkin.SkinStyle = BorderSkinStyle.FrameTitle1;
            chart.Titles.Add(new Title(campaingMetric.Metric.MetricName, Docking.Top, new Font("Trebuchet MS", 14, FontStyle.Bold), this.FontColor));
            chart.Titles.Add(new Title(string.Format(CultureInfo.InvariantCulture, "Días ({0})", month), Docking.Bottom, new Font("Trebuchet MS", 12), this.FontColor));
            chart.Titles.Add(new Title("Valor", Docking.Left, new Font("Trebuchet MS", 12), this.FontColor));

            var chartArea = chart.ChartAreas.Add("Waves");
            var legend = chart.Legends.Add("Legend");

            chartArea.BackColor = this.BackColor;
            legend.BackColor = this.BackColor;

            var series1 = chart.Series.Add("Nivel Optimo");
            var series2 = chart.Series.Add("Nivel Objetivo");
            var series3 = chart.Series.Add("Nivel Mínimo");
            var series4 = chart.Series.Add("Valor Métrica");

            series1.ToolTip = "Nivel Optimo";
            series1.ChartType = SeriesChartType.Line;
            series1.BorderWidth = 2;
            series1.ShadowOffset = 1;
            series1.Color = Color.Green;

            series2.ToolTip = "Nivel Objetivo";
            series2.ChartType = SeriesChartType.Line;
            series2.BorderWidth = 2;
            series2.ShadowOffset = 1;
            series2.Color = Color.Orange;

            series3.ToolTip = "Nivel Mínimo";
            series3.ChartType = SeriesChartType.Line;
            series3.BorderWidth = 2;
            series3.ShadowOffset = 1;
            series3.Color = Color.Red;

            series4.ToolTip = "Valor Métrica";
            series4.ChartType = SeriesChartType.Line;
            series4.BorderWidth = 2;
            series4.ShadowOffset = 1;
            series4.Color = Color.Blue;

            foreach (var date in dates)
            {
                series1.Points.AddXY(date.Day, campaingMetric.OptimalLevel);
                series2.Points.AddXY(date.Day, campaingMetric.ObjectiveLevel);
                series3.Points.AddXY(date.Day, campaingMetric.MinimumLevel);
                series4.Points.AddXY(date.Day, this.metricsRepository.GetUserMetricValue(innerUserId, date, metricId, campaingId));
            }

            var stream = new MemoryStream();
            chart.SaveImage(stream, ChartImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(stream.ToArray(), "image/png");
        }

        private static DateTime GetEndDate(int year, int month)
        {
            return new DateTime(year, month, DateTime.DaysInMonth(year, month)).Date;
        }

        private static DateTime GetEndDate(Campaing campaing, int year, int month)
        {
            if (campaing.EndDate.HasValue && (campaing.EndDate.Value.Year == year) && (campaing.EndDate.Value.Month == month))
            {
                return campaing.EndDate.Value;
            }

            return GetEndDate(year, month);
        }

        private IList<MetricValuesViewModel> CalculateCampaingMetricValues(int innerUserId, int campaingId, DateTime date)
        {
            var campaingMetrics = this.campaingRepository.RetrieveCampaingMetricLevels(campaingId);
            var end = this.GetEndDate(campaingId, date.Year, date.Month);

            date = date > end ? end : date;
                
            return campaingMetrics
                            .Select(cml => new MetricValuesViewModel
                                        {
                                            CampaingId = cml.CampaingId,
                                            MetricId = cml.MetricId,
                                            MetricName = cml.Metric.MetricName,
                                            Format = cml.Metric.Format == 0 
                                                            ? "Porcentual"
                                                            : cml.Metric.Format == 1
                                                                ? "Acumulada"
                                                                : "Diaria",
                                            OptimalValue = cml.OptimalLevel.ToString("F", CultureInfo.InvariantCulture),
                                            ObjectiveValue = cml.ObjectiveLevel.ToString("F", CultureInfo.InvariantCulture),
                                            MinimumValue = cml.MinimumLevel.ToString("F", CultureInfo.InvariantCulture),
                                            CurrentValue = this.metricsRepository.GetUserMetricValue(innerUserId, date, cml.MetricId, campaingId).ToString("F", CultureInfo.InvariantCulture),
                                            ProjectedValue = this.metricsRepository.GetUserMetricValue(innerUserId, end, cml.MetricId, campaingId).ToString("F", CultureInfo.InvariantCulture)
                                        })
                            .ToList();
        }

        private DateTime GetBeginDate(int campaingId, int year, int month)
        {
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);

            if ((campaing.BeginDate.Year == year) && (campaing.BeginDate.Month == month))
            {
                return campaing.BeginDate;
            }

            return new DateTime(year, month, 1);
        }

        private DateTime GetEndDate(int campaingId, int year, int month)
        {
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);

            return GetEndDate(campaing, year, month);
        }

        private SalaryViewModel CalculateSalary(int innerUserId, DateTime date)
        {
            var agent = this.membershipService.RetrieveAgent(innerUserId);
            var campaings = this.campaingRepository.RetrieveCampaingsByUserIdAndDate(innerUserId, date);
            var schedule = this.membershipService.RetrieveMonthlySchedule(innerUserId, date);
            var endDateMonth = GetEndDate(date.Year, date.Month);

            int currentTotalHoursWorked = 0;
            int currentExtraHours50Worked = 0;
            int currentExtraHours100Worked = 0;

            decimal projectedVariableSalary = 0;
            decimal projectedExtra50Salary = 0;
            decimal projectedExtra100Salary = 0;
            decimal projectedTotalSalary = 0;

            int projectedTotalHoursWorked = 0;
            int projectedExtraHours50Worked = 0;
            int projectedExtraHours100Worked = 0;

            decimal gross = 0;

            if (schedule != null)
            {
                gross = schedule.GrossSalary;
                var workday = agent.Workday.Equals("PTE", StringComparison.OrdinalIgnoreCase) ? 120M : 160M;
                var hourlyValue = gross / workday;

                currentTotalHoursWorked = schedule.TotalHoursWorked;
                currentExtraHours50Worked = schedule.ExtraHoursWorked50;
                currentExtraHours100Worked = schedule.ExtraHoursWorked100;

                projectedTotalHoursWorked = (currentTotalHoursWorked * endDateMonth.Day) / date.Day;
                projectedExtraHours50Worked = (currentExtraHours50Worked * endDateMonth.Day) / date.Day;
                projectedExtraHours100Worked = (currentExtraHours100Worked * endDateMonth.Day) / date.Day;

                projectedExtra50Salary = projectedExtraHours50Worked * hourlyValue * 1.5M;
                projectedExtra100Salary = projectedExtraHours100Worked * hourlyValue * 2M;
            }
            else
            {
                var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                format.CurrencySymbol = "$";

                gross = decimal.Parse(agent.GrossSalary, NumberStyles.Any, format);
                projectedTotalHoursWorked = agent.Workday.Equals("PTE", StringComparison.OrdinalIgnoreCase) ? 120 : 160;
                currentTotalHoursWorked = projectedTotalHoursWorked;
            }

            var salaryViewModel = new SalaryViewModel();

            foreach (var campaing in campaings)
            {
                var end = GetEndDate(campaing, date.Year, date.Month);
                var hours = projectedTotalHoursWorked;

                if (endDateMonth.Date != end.Date)
                {
                    hours = (campaing.EndDate.Value.Day * projectedTotalHoursWorked) / endDateMonth.Day;
                }

                var metricLevel = this.CalculateMetricsLevel(innerUserId, campaing.Id, date, end, false);

                if (metricLevel == MetricLevel.Optimal) { projectedVariableSalary += campaing.OptimalHourlyValue * hours; }
                if (metricLevel == MetricLevel.Objective) { projectedVariableSalary += campaing.ObjectiveHourlyValue * hours; }
                if (metricLevel == MetricLevel.Objective) { projectedVariableSalary += campaing.MinimumHourlyValue * hours; }
            }

            projectedTotalSalary = gross + projectedVariableSalary + projectedExtra50Salary + projectedExtra100Salary;

            salaryViewModel.GrossSalary = gross.ToString("C", CultureInfo.CurrentUICulture);
            salaryViewModel.VariableSalary = projectedVariableSalary.ToString("C", CultureInfo.CurrentUICulture);
            salaryViewModel.Extra50Salary = projectedExtra50Salary.ToString("C", CultureInfo.CurrentUICulture);
            salaryViewModel.Extra100Salary = projectedExtra100Salary.ToString("C", CultureInfo.CurrentUICulture);
            salaryViewModel.TotalSalary = projectedTotalSalary.ToString("C", CultureInfo.CurrentUICulture);
            salaryViewModel.TotalHoursWorked = projectedTotalHoursWorked;
            salaryViewModel.CurrentExtraHours50Worked = currentExtraHours50Worked;
            salaryViewModel.CurrentExtraHours100Worked = currentExtraHours100Worked;
            salaryViewModel.ExtraHours50Worked = projectedExtraHours50Worked;
            salaryViewModel.ExtraHours100Worked = projectedExtraHours100Worked;

            return salaryViewModel;
        }

        private MetricLevel CalculateMetricsLevel(int innerUserId, int campaingId, DateTime currentDate, DateTime projectedDate, bool currentMetricLevel)
        {
            var optimalCount = 0;
            var objectiveCount = 0;
            var minimumCount = 0;
            var campaingMetrics = this.campaingRepository
                                            .RetrieveCampaingMetricLevels(campaingId)
                                            .Select(cml => new
                                            {
                                                IsHighestToLowest = cml.Metric.IsHighestToLowest,
                                                OptimalValue = cml.OptimalLevel,
                                                ObjectiveValue = cml.ObjectiveLevel,
                                                MinimumValue = cml.MinimumLevel,
                                                CurrentValue = this.metricsRepository.GetUserMetricValue(innerUserId, currentDate, cml.MetricId, campaingId),
                                                ProjectedValue = this.metricsRepository.GetUserMetricValue(innerUserId, projectedDate, cml.MetricId, campaingId)
                                            });
            
            foreach (var metricValue in campaingMetrics)
            {
                var value = currentMetricLevel ? metricValue.CurrentValue : metricValue.ProjectedValue;

                if (metricValue.IsHighestToLowest)
                {
                    if (metricValue.OptimalValue <= Math.Round(value, 2)) { optimalCount++; }
                    if (metricValue.ObjectiveValue <= Math.Round(value, 2)) { objectiveCount++; }
                    if (metricValue.MinimumValue <= Math.Round(value, 2)) { minimumCount++; }
                }
                else
                {
                    if (metricValue.OptimalValue >= Math.Round(value, 2)) { optimalCount++; }
                    if (metricValue.ObjectiveValue >= Math.Round(value, 2)) { objectiveCount++; }
                    if (metricValue.MinimumValue >= Math.Round(value, 2)) { minimumCount++; }
                }
            }

            if (optimalCount == campaingMetrics.Count()) { return MetricLevel.Optimal; }
            if (objectiveCount == campaingMetrics.Count()) { return MetricLevel.Objective; }
            if (minimumCount == campaingMetrics.Count()) { return MetricLevel.Minimum; }

            return MetricLevel.Unsatisfactory;
        }

        private Agent GetAgent(int? pageNumber, int? campaingId, int? supervisorId, out bool shoulPaginate, out bool shoulIncludeCampaing, out bool shoulIncludeSupervisor, out int page, out int totalCount)
        {
            shoulIncludeCampaing = false;
            shoulIncludeSupervisor = false;
            if (User.IsInRole("AccountManager"))
            {
                shoulPaginate = true;
                shoulIncludeCampaing = campaingId.HasValue;
                shoulIncludeSupervisor = !campaingId.HasValue && supervisorId.HasValue;
                totalCount = campaingId.HasValue
                                ? this.campaingRepository.CountCampaingAgents(campaingId.Value)
                                : supervisorId.HasValue
                                    ? this.membershipService.CountAgentsBySupervisorId(supervisorId.Value)
                                    : this.membershipService.CountAllAgents();

                if (totalCount == 0)
                {
                    page = 0;
                    return null;
                }

                page = !pageNumber.HasValue
                            ? 1
                            : pageNumber.Value > totalCount
                                ? totalCount
                                : pageNumber.Value;

                return campaingId.HasValue
                          ? this.campaingRepository.RetrieveCampaingAgents(campaingId.Value, 1, page).FirstOrDefault()
                          : supervisorId.HasValue
                                ? this.membershipService.RetrieveAgentsBySupervisorId(supervisorId.Value, 1, page).FirstOrDefault()
                                : this.membershipService.RetrieveAllAgents(1, page).FirstOrDefault();
            }

            if (User.IsInRole("Supervisor"))
            {
                var currentSupervisorId = this.membershipService.RetrieveSupervisor(this.User.Identity.Name).InnerUserId;
                shoulPaginate = true;
                totalCount = this.membershipService.CountAgentsBySupervisorId(currentSupervisorId);
                
                if (totalCount == 0)
                {
                    page = 0;
                    return null;
                }

                page = !pageNumber.HasValue
                            ? 1
                            : pageNumber.Value > totalCount
                                ? totalCount
                                : pageNumber.Value;

                return this.membershipService.RetrieveAgentsBySupervisorId(currentSupervisorId, 1, page).FirstOrDefault();
            }

            shoulPaginate = false;
            totalCount = 1;
            page = 1;

            return this.membershipService.RetrieveAgent(this.User.Identity.Name);
        }

        private Agent SearchAgent(string searchCriteria, int? pageNumber, out bool shoulPaginate, out int page, out int totalCount)
        {
            IList<Agent> agents;
            if (User.IsInRole("AccountManager"))
            {
                var agentId = 0;
                if (int.TryParse(searchCriteria, NumberStyles.Integer, CultureInfo.InvariantCulture, out agentId))
                {
                    var agent = this.membershipService.RetrieveAgent(agentId);
                    if (agent != null)
                    {
                        shoulPaginate = false;
                        page = 1;
                        totalCount = 1;

                        return agent;
                    }
                }

                agents = this.membershipService.SearchAgents(searchCriteria);
            }
            else
            {
                var agentId = 0;
                var currentSupervisorId = this.membershipService.RetrieveSupervisor(this.User.Identity.Name).InnerUserId;
                if (int.TryParse(searchCriteria, NumberStyles.Integer, CultureInfo.InvariantCulture, out agentId))
                {
                    var agent = this.membershipService.RetrieveAgent(agentId);
                    if ((agent != null) && (agent.SupervisorId == currentSupervisorId))
                    {
                        shoulPaginate = false;
                        page = 1;
                        totalCount = 1;

                        return agent;
                    }
                }

                agents = this.membershipService.SearchAgentsBySupervisorId(currentSupervisorId, searchCriteria);
            }

            shoulPaginate = true;
            totalCount = agents.Count;
            page = !pageNumber.HasValue
                        ? 1
                        : pageNumber.Value > totalCount
                            ? totalCount
                            : pageNumber.Value;
            
            return agents.Skip(page - 1).Take(1).FirstOrDefault();
        }
    }
}
