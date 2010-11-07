namespace CallCenter.SelfManagement.Web.Controllers
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

        //
        // GET: /Agent/
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Index()
        {
            // TODO: implement this action for AccountManager and Supervisor roles
            if (User.IsInRole("AccountManager") || User.IsInRole("Supervisor"))
            {
                return this.View("IndexToDo");
            }

            var agent = this.membershipService.RetrieveAgent(this.User.Identity.Name);
            var currentSupervisor = this.membershipService.RetrieveSupervisor(agent.SupervisorId.Value);
            var userCampaing = this.campaingRepository.RetrieveCurrentCampaingByUserId(agent.InnerUserId);
            var userCampaings = this.campaingRepository.RetrieveCampaingsByUserId(agent.InnerUserId);                                        

            if (userCampaing == null)
            {
                userCampaing = userCampaings.OrderByDescending(c => c.BeginDate).LastOrDefault();
            }

            var model = new AgentDetailsViewModel
            {
                AgentId = agent.InnerUserId,
                AvailableSalaryMonths = this.membershipService.RetrieveAvailableMonthsByUser(agent.InnerUserId),
                AvailableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id),
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", agent.Name, agent.LastName, agent.InnerUserId),
                CurrentSupervisor = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", currentSupervisor.Name, currentSupervisor.LastName, currentSupervisor.InnerUserId),
                CurrentCampaingId = userCampaing != null ? userCampaing.Id : 0,
                AgentCampaings = userCampaings.Select(c => c.ToUserCampaingInfo()).ToList(),
                OptimalHourlyValue = userCampaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                ObjectiveHourlyValue = userCampaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                MinimumHourlyValue = userCampaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture)
            };

            var today = DateTime.Now;
            model.CurrentCampaingMetricValues = this.CalculateCampaingMetricValues(agent.InnerUserId, model.CurrentCampaingId, today);
            model.Salary = this.CalculateSalary(agent.InnerUserId, today);

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
            var model = this.CalculateCampaingMetricValues(innerUserId, campaingId, DateTime.Now);
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
            var availableMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(campaingId);
            var today = DateTime.Now.Date;
            var monthIndex = availableMonths.IndexOf(today.ToString("yyyy-MM"));

            if (monthIndex == -1)
            {
                var last = availableMonths.Last().Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                var lastYear = int.Parse(last[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
                var lastMonth = int.Parse(last[1], NumberStyles.Integer, CultureInfo.InvariantCulture);

                if ((today.Year > lastYear) || ((today.Year == lastYear) && (today.Month > lastMonth)))
                {
                    monthIndex = availableMonths.Count - 1;
                }
                
                monthIndex = 0;
            }

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
                                CurrentMetricMonthIndex = monthIndex
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
                BorderlineColor = FontColor,
                BackSecondaryColor = BackColor,
                BackColor = BackColor,
                ForeColor = FontColor,
            };

            chart.BorderSkin.SkinStyle = BorderSkinStyle.FrameTitle1;
            chart.Titles.Add(new Title(campaingMetric.Metric.MetricName, Docking.Top, new Font("Trebuchet MS", 14, FontStyle.Bold), FontColor));
            chart.Titles.Add(new Title(string.Format(CultureInfo.InvariantCulture, "Días ({0})", month), Docking.Bottom, new Font("Trebuchet MS", 12), FontColor));
            chart.Titles.Add(new Title("Valor", Docking.Left, new Font("Trebuchet MS", 12), FontColor));
            
            var chartArea = chart.ChartAreas.Add("Waves");
            var legend = chart.Legends.Add("Legend");

            chartArea.BackColor = BackColor;
            legend.BackColor = BackColor;

            var series1 = chart.Series.Add("Nivel Optimo");
            var series2 = chart.Series.Add("Nivel Objetivo");
            var series3 = chart.Series.Add("Nivel Mínimo");
            var series4 = chart.Series.Add("Valor Métrica");

            series1.ToolTip = "Nivel Optimo";
            series1.ChartType = SeriesChartType.Line;
            series1.BorderWidth = 3;
            series1.ShadowOffset = 2;
            series1.Color = Color.Green;

            series2.ToolTip = "Nivel Objetivo";
            series2.ChartType = SeriesChartType.Line;
            series2.BorderWidth = 3;
            series2.ShadowOffset = 2;
            series2.Color = Color.Yellow;
                        
            series3.ToolTip = "Nivel Mínimo";
            series3.ChartType = SeriesChartType.Line;
            series3.BorderWidth = 3;
            series3.ShadowOffset = 2;
            series3.Color = Color.Red;

            series4.ToolTip = "Valor Métrica";
            series4.ChartType = SeriesChartType.Line;
            series4.BorderWidth = 3;
            series4.ShadowOffset = 2;
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

        //
        // GET: /Agent/Create
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Create()
        {
            return this.View();
        } 

        //
        // POST: /Agent/Create
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.View();
            }
        }
        
        //
        // GET: /Agent/Edit/5
        [Authorize(Roles = "AccountManager, Supervisor, Agent")] 
        public ActionResult Edit(int id)
        {
            return this.View();
        }

        //
        // POST: /Agent/Edit/5
        [HttpPost]
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.View();
            }
        }

        //
        // GET: /Agent/Delete/5
        [Authorize(Roles = "AccountManager, Supervisor, Agent")] 
        public ActionResult Delete(int id)
        {
            return this.View();
        }

        //
        // POST: /Agent/Delete/5

        [HttpPost]
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return this.RedirectToAction("Index");
            }
            catch
            {
                return this.View();
            }
        }

        private static DateTime GetEndDate(int year, int month)
        {
            if ((month == 1) || (month == 3) || (month == 5) || (month == 7) || (month == 8) || (month == 10) || (month == 12))
            {
                return new DateTime(year, month, 31);
            }

            if (month == 2)
            {
                return DateTime.IsLeapYear(month) ? new DateTime(year, month, 29) : new DateTime(year, month, 28);
            }

            return new DateTime(year, month, 30);
        }

        private IList<MetricValuesViewModel> CalculateCampaingMetricValues(int innerUserId, int campaingId, DateTime date)
        {
            var campaingMetrics = this.campaingRepository.RetrieveCampaingMetricLevels(campaingId);
            var end = this.GetEndDate(campaingId, date.Year, date.Month);

            return campaingMetrics
                            .Select(cml => new MetricValuesViewModel
                                        {
                                            CampaingId = cml.CampaingId,
                                            MetricId = cml.MetricId,
                                            MetricName = cml.Metric.MetricName,
                                            Format = cml.Metric.Format == 0 ? "Porcentual" : "Acumulada",
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

            if (campaing.EndDate.HasValue && (campaing.EndDate.Value.Year == year) && (campaing.EndDate.Value.Month == month))
            {
                return campaing.EndDate.Value;
            }

            return GetEndDate(year, month);
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
                gross = decimal.Parse(agent.GrossSalary, NumberStyles.Any, CultureInfo.InvariantCulture);
                projectedTotalHoursWorked = agent.Workday.Equals("PTE", StringComparison.OrdinalIgnoreCase) ? 120 : 160;
                currentTotalHoursWorked = projectedTotalHoursWorked;
            }

            var salaryViewModel = new SalaryViewModel();

            foreach (var campaing in campaings)
            {
                var end = this.GetEndDate(campaing.Id, date.Year, date.Month);
                var campaingMetrics = this.campaingRepository
                                            .RetrieveCampaingMetricLevels(campaing.Id)
                                            .Select(cml => new
                                                {
                                                    OptimalValue = cml.OptimalLevel,
                                                    ObjectiveValue = cml.ObjectiveLevel,
                                                    MinimumValue = cml.MinimumLevel,
                                                    CurrentValue = this.metricsRepository.GetUserMetricValue(innerUserId, date, cml.MetricId, campaing.Id),
                                                    ProjectedValue = this.metricsRepository.GetUserMetricValue(innerUserId, end, cml.MetricId, campaing.Id)
                                                });
                var optimalCount = 0;
                var objectiveCount = 0;
                var minimumCount = 0;
                var hours = projectedTotalHoursWorked;

                if (endDateMonth.Date != end.Date)
                {
                    hours = (campaing.EndDate.Value.Day * projectedTotalHoursWorked) / endDateMonth.Day;
                }
                
                foreach (var metricValue in campaingMetrics)
                {
                    if (metricValue.OptimalValue <= metricValue.ProjectedValue) { optimalCount++; }
                    if (metricValue.ObjectiveValue <= metricValue.ProjectedValue) { objectiveCount++; }
                    if (metricValue.MinimumValue <= metricValue.ProjectedValue) { minimumCount++; }
                }

                if (optimalCount == campaingMetrics.Count()) { projectedVariableSalary += campaing.OptimalHourlyValue * hours; }
                if (objectiveCount == campaingMetrics.Count()) { projectedVariableSalary += campaing.ObjectiveHourlyValue * hours; }
                if (minimumCount == campaingMetrics.Count()) { projectedVariableSalary += campaing.MinimumHourlyValue * hours; }
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
    }
}
