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
                AvailableMonths = this.membershipService.RetrieveAvailableMonthsByUser(agent.InnerUserId),
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", agent.Name, agent.LastName, agent.InnerUserId),
                CurrentSupervisor = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", currentSupervisor.Name, currentSupervisor.LastName, currentSupervisor.InnerUserId),
                CurrentCampaingId = currentCampaing != null ? currentCampaing.Id : 0,
                AgentCampaings = this.campaingRepository
                                        .RetrieveCampaingsByUserId(agent.InnerUserId)
                                        .Select(c => c.ToUserCampaingInfo())
                                        .ToList(),
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
            // TODO: Refactor this to receive a date range or month
            var model = this.CalculateCampaingMetricValues(innerUserId, campaingId, DateTime.Now);
            
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { CampaingMetricValues = model } };
        }



        private Font font = new Font("Trebuchet MS", 14, FontStyle.Bold);
        private Color color = Color.FromArgb(26, 59, 105);

        public ActionResult MetricsChart()
        {
            // Define the Data ... this simple example is just a list of values from 0 to 50
            List<float> values = new List<float>();
            for (int i = 0; i < 50; i++)
            {
                values.Add(i);
            }

            // Define the Chart
            Chart Chart2 = new Chart()
            {
                Width = 952,
                Height = 350,
                RenderType = RenderType.BinaryStreaming,
                Palette = ChartColorPalette.BrightPastel,
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderWidth = 2,
                BorderColor = color
            };
            Chart2.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            Chart2.Titles.Add(new Title("TODO", Docking.Top, font, color));
            Chart2.ChartAreas.Add("Waves");
            Chart2.Legends.Add("Legend");

            //Bind the model data to the chart
            var series1 = Chart2.Series.Add("Sin");
            var series2 = Chart2.Series.Add("Cos");

            foreach (int value in values)
            {
                series1.Points.AddY((float)Math.Sin(value * .5) * 100f);
            }

            foreach (int value in values)
            {
                series2.Points.AddY((float)Math.Cos(value * .5) * 100f);
            }

            // Stream the image to the browser
            MemoryStream stream = new MemoryStream();
            Chart2.SaveImage(stream, ChartImageFormat.Png);
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

        private IList<MetricValuesViewModel> CalculateCampaingMetricValues(int innerUserId ,int campaingId, DateTime date)
        {
            var campaingMetrics = this.campaingRepository.RetrieveCampaingMetricLevels(campaingId);
            var end = this.GetEndDate(campaingId);

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

        private DateTime GetEndDate(int campaingId)
        {
            var today = DateTime.Now;
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);

            if (campaing.EndDate.HasValue && (campaing.EndDate.Value.Year == today.Year) && (campaing.EndDate.Value.Month == today.Month))
            {
                return campaing.EndDate.Value;
            }
            
            if ((today.Month == 1) || (today.Month == 3) || (today.Month == 5) || (today.Month == 7) || (today.Month == 8) || (today.Month == 10) || (today.Month == 12))
            {
                return today.AddDays(31 - today.Day);
            }

            if (today.Month == 2)
            {
                return today.AddDays(28 - today.Day);
            }

            return today.AddDays(30 - today.Day);
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
