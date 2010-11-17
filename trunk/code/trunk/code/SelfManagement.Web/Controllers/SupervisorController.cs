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

    public class SupervisorController : Controller
    {
        private readonly Color FontColor = Color.FromArgb(51, 93, 76);
        private readonly Color BackColor = Color.FromArgb(221, 221, 238);
        private readonly ICampaingRepository campaingRepository;
        private readonly IMetricsRepository metricsRepository;
        private readonly IMembershipService membershipService;

        public SupervisorController()
            : this(new RepositoryFactory().GetCampaingRepository(), new RepositoryFactory().GetMetricsRepository(), new RepositoryFactory().GetMembershipService())
        {
        }

        public SupervisorController(ICampaingRepository campaingRepository, IMetricsRepository metricsRepository, IMembershipService membershipService)
        {
            this.campaingRepository = campaingRepository;
            this.metricsRepository = metricsRepository;
            this.membershipService = membershipService;
        }

        //
        // GET: /Supervisor/
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Index(int? pageNumber, int? campaingId)
        {
            bool shoulPaginate;
            bool shouldIncludeCampaing;
            int totalCount;
            int page;
            var supervisor = this.GetSupervisor(pageNumber, campaingId, out shoulPaginate, out shouldIncludeCampaing, out page, out totalCount);

            var userCampaing = (campaingId.HasValue) ? this.campaingRepository.RetrieveCampaingById(campaingId.GetValueOrDefault(0)) : this.campaingRepository.RetrieveCurrentCampaingByUserId(supervisor.InnerUserId);
            var userCampaings = this.campaingRepository.RetrieveCampaingsByUserId(supervisor.InnerUserId);

            DateTime metricsDate;
            IList<string> availableMetricMonths;
            if (userCampaing == null)
            {
                userCampaing = userCampaings.OrderByDescending(c => c.BeginDate).LastOrDefault();
                availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);

                var date = availableMetricMonths.LastOrDefault();

                metricsDate = DateTime.ParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
                metricsDate = GetEndDate(metricsDate.Year, metricsDate.Month);
            }
            else
            {
                if (campaingId.HasValue)
                {
                    availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);

                    var date = availableMetricMonths.LastOrDefault();

                    metricsDate = DateTime.ParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None).Date;
                    metricsDate = GetEndDate(metricsDate.Year, metricsDate.Month);
                }
                else
                {
                    metricsDate = DateTime.Now;
                    availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(userCampaing.Id);
                }
            }

            var model = new SupervisorDetailsViewModel
            {
                SupervisorId = supervisor.InnerUserId,
                AvailableMetricMonths = availableMetricMonths,
                DisplayName = EntityTranslator.GetSupervisorDisplayName(supervisor),
                AgentsCount = string.Format(CultureInfo.InvariantCulture, "{0} (ver todos)", this.membershipService.CountAgentsBySupervisorId(supervisor.InnerUserId)),
                CurrentCampaingId = userCampaing.Id,
                SupervisorCampaings = userCampaings.Select(c => c.ToUserCampaingInfo()).ToList(),
                CurrentMetricLevel = this.CalculateMetricsLevel(supervisor.InnerUserId, userCampaing.Id, metricsDate, GetEndDate(metricsDate.Year, metricsDate.Month), true),
                ProjectedMetricLevel = this.CalculateMetricsLevel(supervisor.InnerUserId, userCampaing.Id, metricsDate, GetEndDate(metricsDate.Year, metricsDate.Month), false),
                CurrentCampaingMetricValues = this.CalculateCampaingMetricValues(supervisor.InnerUserId, userCampaing.Id, metricsDate),
                ShouldPaginate = shoulPaginate,
                ShouldIncludeCampaing = shouldIncludeCampaing,
                CampaingIdForPagination = shouldIncludeCampaing ? campaingId.Value : 0,
                PageNumber = page,
                TotalPages = totalCount
            };

            return this.View(model);
        }

        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult CampaingMetricValues(int innerUserId, int campaingId)
        {
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
            var availableMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(campaingId);
            var today = DateTime.Now.Date;
            var monthIndex = availableMonths.IndexOf(today.ToString("yyyy-MM"));
            DateTime date = today;

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

                date = flag ? GetEndDate(metricsDate.Year, metricsDate.Month) : metricsDate;
            }

            var model = this.CalculateCampaingMetricValues(innerUserId, campaingId, date);
            var currentMetricLevel = this.CalculateMetricsLevel(innerUserId, campaing.Id, date, GetEndDate(date.Year, date.Month), true);
            var projectedMetricLevel = this.CalculateMetricsLevel(innerUserId, campaing.Id, date, GetEndDate(date.Year, date.Month), false);

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    CampaingMetricValues = model,
                    AvailableMetricMonths = availableMonths,
                    CurrentMetricMonthIndex = monthIndex,
                    CurrentMetricLevelDescription = currentMetricLevel.GetDescription(),
                    ProjectedMetricLevelDescription = projectedMetricLevel.GetDescription(),
                    CurrentMetricLevelCssClass = currentMetricLevel.GetCssClass(),
                    ProjectedMetricLevelCssClass = projectedMetricLevel.GetCssClass()
                }
            };
        }

        [Authorize(Roles = "AccountManager, Supervisor")]
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
            series2.Color = Color.YellowGreen;

            series3.ToolTip = "Nivel Mínimo";
            series3.ChartType = SeriesChartType.Line;
            series3.BorderWidth = 3;
            series3.ShadowOffset = 2;
            series3.Color = Color.Orange;

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
        // GET: /Supervisor/Create
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Supervisor/Create
        [HttpPost]
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Supervisor/Edit/5
        [Authorize(Roles = "AccountManager, Supervisor")] 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Supervisor/Edit/5
        [HttpPost]
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Supervisor/Delete/5
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Supervisor/Delete/5
        [HttpPost]
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        private static DateTime GetEndDate(int year, int month)
        {
            return new DateTime(year, month, DateTime.DaysInMonth(year, month)).Date;
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

            if (campaing.EndDate.HasValue && (campaing.EndDate.Value.Year == year) && (campaing.EndDate.Value.Month == month))
            {
                return campaing.EndDate.Value;
            }

            return GetEndDate(year, month);
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
                    if (metricValue.OptimalValue <= value) { optimalCount++; }
                    if (metricValue.ObjectiveValue <= value) { objectiveCount++; }
                    if (metricValue.MinimumValue <= value) { minimumCount++; }
                }
                else
                {
                    if (metricValue.OptimalValue >= value) { optimalCount++; }
                    if (metricValue.ObjectiveValue >= value) { objectiveCount++; }
                    if (metricValue.MinimumValue >= value) { minimumCount++; }
                }
            }

            if (optimalCount == campaingMetrics.Count()) { return MetricLevel.Optimal; }
            if (objectiveCount == campaingMetrics.Count()) { return MetricLevel.Objective; }
            if (minimumCount == campaingMetrics.Count()) { return MetricLevel.Minimum; }

            return MetricLevel.Unsatisfactory;
        }


        private Supervisor GetSupervisor(int? pageNumber, int? campaingId, out bool shoulPaginate, out bool shoulIncludeCampaing, out int page, out int totalCount)
        {
            if (User.IsInRole("AccountManager"))
            {
                shoulPaginate = true;
                shoulIncludeCampaing = campaingId.HasValue;
                totalCount = !campaingId.HasValue
                                ? this.membershipService.CountAllSupervisors()
                                : this.campaingRepository.CountCampaingSupervisors(campaingId.Value);
                page = !pageNumber.HasValue
                            ? 1
                            : pageNumber.Value > totalCount
                                ? totalCount
                                : pageNumber.Value;

                return !campaingId.HasValue
                          ? this.membershipService.RetrieveAllSupervisors(1, page).FirstOrDefault()
                          : this.campaingRepository.RetrieveCampaingSupervisors(campaingId.Value, 1, page).FirstOrDefault();
            }

            shoulIncludeCampaing = false;
            shoulPaginate = false;
            totalCount = 1;
            page = 1;

            return this.membershipService.RetrieveSupervisor(this.User.Identity.Name);
        }
    }
}
