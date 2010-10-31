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
        private readonly Font Font = new Font("Trebuchet MS", 14, FontStyle.Bold);
        private readonly Color Color = Color.FromArgb(26, 59, 105);

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
            var currentCampaing = this.campaingRepository.RetrieveUserCurrentCampaing(agent.InnerUserId);
            var model = new AgentDetailsViewModel
            {
                AgentId = agent.InnerUserId,
                AvailableSalaryMonths = this.membershipService.RetrieveAvailableMonthsByUser(agent.InnerUserId),
                AvailableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(currentCampaing.Id),
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", agent.Name, agent.LastName, agent.InnerUserId),
                CurrentSupervisor = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", currentSupervisor.Name, currentSupervisor.LastName, currentSupervisor.InnerUserId),
                CurrentCampaingId = currentCampaing != null ? currentCampaing.Id : 0,
                AgentCampaings = this.campaingRepository
                                        .RetrieveCampaingsByUserId(agent.InnerUserId)
                                        .Select(c => c.ToUserCampaingInfo())
                                        .ToList(),
                OptimalHourlyValue = currentCampaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                ObjectiveHourlyValue = currentCampaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                MinimumHourlyValue = currentCampaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture)
            };

            model.CurrentCampaingMetricValues = this.CalculateCampaingMetricValues(agent.InnerUserId, model.CurrentCampaingId, DateTime.Now);
            model.Salary = this.CalculateSalary(agent.InnerUserId, model.CurrentCampaingId, model.CurrentCampaingMetricValues);

            return this.View(model);
        }

        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Salary(int innerUserId, string month)
        {
            var date = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None);
            var campaing = this.campaingRepository.RetrieveCampaingsByUserIdAndDate(innerUserId, date).FirstOrDefault();

            if (campaing != null)
            {
                var campaingMetricValues = this.CalculateCampaingMetricValues(innerUserId, campaing.Id, date);
                var model = this.CalculateSalary(innerUserId, campaing.Id, campaingMetricValues);

                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { Salary = model } };
            }

            var grossSalary = this.membershipService.RetrieveAgent(innerUserId).GrossSalary;
            var emptyModel = new SalaryViewModel { GrossSalary = "$" + grossSalary, VariableSalary = "$0", TotalSalary = "$" + grossSalary };

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { Salary = emptyModel } };           
        }

        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult CampaingMetricValues(int innerUserId, int campaingId)
        {
            var model = this.CalculateCampaingMetricValues(innerUserId, campaingId, DateTime.Now);
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
            var availableMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(campaingId);

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
                                CurrentMetricMonthIndex = availableMonths.Count - 1
                            }
                };
        }

        // TODO: update this method.
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult MetricsChart(int innerUserId, int campaingId, int metricId, string month)
        {
            var beginDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
            var endDate = this.GetEndDate(campaingId, beginDate.Year, beginDate.Month).Date;
            var campaingMetric = this.campaingRepository.RetrieveCampaingMetricLevels(campaingId).FirstOrDefault(cml => cml.MetricId == metricId);
            var dates = new List<DateTime>();

            while (beginDate <= endDate)
            {
                beginDate = beginDate.AddDays(1);
                dates.Add(beginDate);
            }

            var chart = new Chart()
            {
                Width = 952,
                Height = 350,
                RenderType = RenderType.BinaryStreaming,
                Palette = ChartColorPalette.BrightPastel,
                // Solid
                BorderlineDashStyle = ChartDashStyle.NotSet,
                ToolTip = campaingMetric.Metric.MetricName,
                BorderWidth = 2,
                BorderColor = Color,
                // Remove
                BorderlineWidth = 0,
            };

            chart.BorderSkin.SkinStyle = BorderSkinStyle.None;
            chart.BorderSkin.BorderWidth = 0;
            chart.Titles.Add(new Title(campaingMetric.Metric.MetricName, Docking.Top, Font, Color));
            chart.Titles.Add(new Title("Días", Docking.Bottom));
            chart.Titles.Add(new Title("Valor", Docking.Left));
            
            var chartArea = chart.ChartAreas.Add("Waves");
            var legend = chart.Legends.Add("Legend");

            chartArea.BorderDashStyle = ChartDashStyle.NotSet;
            
            var series1 = chart.Series.Add("Nivel Óptimo");
            var series2 = chart.Series.Add("Nivel Objectivo");
            var series3 = chart.Series.Add("Nivel Mínimo");
            var series4 = chart.Series.Add("Valor Métrica");

            series1.ToolTip = "Nivel Optimo";
            series1.ChartType = SeriesChartType.Line;
            series1.BorderWidth = 2;
            series2.ToolTip = "Nivel Objectivo";
            series2.ChartType = SeriesChartType.Line;
            series2.BorderWidth = 2;
            series3.ToolTip = "Nivel Mínimo";
            series3.ChartType = SeriesChartType.Line;
            series3.BorderWidth = 2;
            series4.ToolTip = "Valor Métrica";
            series4.ChartType = SeriesChartType.Line;
            series4.BorderWidth = 2;

            foreach (var date in dates)
            {
                series1.Points.AddY(campaingMetric.OptimalLevel);
                series2.Points.AddY(campaingMetric.ObjectiveLevel);
                series3.Points.AddY(campaingMetric.MinimumLevel);
                series4.Points.AddY(this.metricsRepository.GetUserMetricValue(innerUserId, date, metricId, campaingId));
            }

            // Stream the image to the browser
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

        private DateTime GetEndDate(int campaingId, int year, int month)
        {
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);

            if (campaing.EndDate.HasValue && (campaing.EndDate.Value.Year == year) && (campaing.EndDate.Value.Month == month))
            {
                return campaing.EndDate.Value;
            }

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

        private SalaryViewModel CalculateSalary(int innerUserId, int campaingId, IList<MetricValuesViewModel> metricValues)
        {
            var agent = this.membershipService.RetrieveAgent(innerUserId);
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
            var hours = agent.Workday.Equals("PTE", StringComparison.OrdinalIgnoreCase) ? 120 : 160;
            var salaryViewModel = new SalaryViewModel();

            var optimalCount = 0;
            var objectiveCount = 0;
            var minimumCount = 0;

            foreach (var metricValue in metricValues)
            {
                var optimal = double.Parse(metricValue.OptimalValue, NumberStyles.Any, CultureInfo.InvariantCulture);
                var objective = double.Parse(metricValue.ObjectiveValue, NumberStyles.Any, CultureInfo.InvariantCulture);
                var minimum = double.Parse(metricValue.MinimumValue, NumberStyles.Any, CultureInfo.InvariantCulture);
                var projected = double.Parse(metricValue.ProjectedValue, NumberStyles.Any, CultureInfo.InvariantCulture);

                if (optimal <= projected) { optimalCount++; }
                if (objective <= projected) { objectiveCount++; }
                if (minimum <= projected) { minimumCount++; }
            }

            var gross = double.Parse(agent.GrossSalary, NumberStyles.Any, CultureInfo.InvariantCulture);
            salaryViewModel.GrossSalary = gross.ToString("C", CultureInfo.CurrentUICulture);

            if (optimalCount == metricValues.Count)
            {
                var variable = campaing.OptimalHourlyValue * hours;

                salaryViewModel.VariableSalary = variable.ToString("C", CultureInfo.CurrentUICulture);
                salaryViewModel.TotalSalary = (gross + Convert.ToDouble(variable)).ToString("C", CultureInfo.CurrentUICulture);

                return salaryViewModel;
            }

            if (objectiveCount == metricValues.Count)
            {
                var variable = campaing.ObjectiveHourlyValue * hours;

                salaryViewModel.VariableSalary = variable.ToString("C", CultureInfo.CurrentUICulture);
                salaryViewModel.TotalSalary = (gross + Convert.ToDouble(variable)).ToString("C", CultureInfo.CurrentUICulture);

                return salaryViewModel;
            }

            if (minimumCount == metricValues.Count)
            {
                var variable = campaing.MinimumHourlyValue * hours;

                salaryViewModel.VariableSalary = variable.ToString("C", CultureInfo.CurrentUICulture);
                salaryViewModel.TotalSalary = (gross + Convert.ToDouble(variable)).ToString("F", CultureInfo.CurrentUICulture);

                return salaryViewModel;
            }

            salaryViewModel.VariableSalary = "0";
            salaryViewModel.TotalSalary = salaryViewModel.GrossSalary;

            return salaryViewModel;
        }
    }
}
