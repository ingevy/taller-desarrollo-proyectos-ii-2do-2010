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
    
    public class CampaingController : Controller
    {
        private const string CampaingMetricLevelsKey = "CampaingMetricLevels[{0}].{1}";

        private readonly Color FontColor = Color.FromArgb(51, 93, 76);
        private readonly Color BackColor = Color.FromArgb(221, 221, 238);
        private readonly ICampaingRepository campaingRepository;
        private readonly IMetricsRepository metricsRepository;
        private readonly IMembershipService membershipService;

        public CampaingController()
            : this(new RepositoryFactory().GetCampaingRepository(), new RepositoryFactory().GetMetricsRepository(), new RepositoryFactory().GetMembershipService())
        {
        }

        public CampaingController(ICampaingRepository campaingRepository, IMetricsRepository metricsRepository, IMembershipService membershipService)
        {
            this.campaingRepository = campaingRepository;
            this.metricsRepository = metricsRepository;
            this.membershipService = membershipService;
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult Index(int? pageNumber)
        {
            int totalCount;
            int page;
            var campaing = this.GetCampaing(pageNumber, out page, out totalCount);

            if (campaing == null)
            {
                return this.View("NotFound", new CampaingDetailsViewModel());
            }

            return this.View(CreateCampaingDetailsViewModel(campaing, null, true, page, totalCount));
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult Search(string searchCriteria, int? pageNumber)
        {
            if (string.IsNullOrWhiteSpace(searchCriteria))
            {
                return this.RedirectToAction("Index", "Campaing");
            }

            int totalCount;
            int page;
            bool shouldPaginate;
            searchCriteria = searchCriteria.Trim();
            var campaing = this.SearchCampaing(searchCriteria, pageNumber, out shouldPaginate, out page, out totalCount);

            if (campaing == null)
            {
                return this.View("NotFound", new CampaingDetailsViewModel { SearchCriteria = searchCriteria });
            }

            return this.View(CreateCampaingDetailsViewModel(campaing, searchCriteria, shouldPaginate, page, totalCount));
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult MetricsChart(int campaingId, int metricId, string month)
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
                series4.Points.AddXY(date.Day, this.metricsRepository.GetCampaingMetricValue(campaingId, date, metricId));
            }

            var stream = new MemoryStream();
            chart.SaveImage(stream, ChartImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(stream.ToArray(), "image/png");
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult Create()
        {
            var model = new CampaingViewModel
            {
                Id = 0,
                IsEditing = false,
                BeginDate = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.CurrentUICulture),
                CampaingSupervisors = this.campaingRepository
                                            .RetrieveAvailableSupervisors(DateTime.Today)
                                            .Select(up => up.ToViewModel())
                                            .OrderByDescending(s => s.Selected)
                                            .ThenBy(s => s.Id)
                                            .ToList(),
                CampaingMetricLevels = this.campaingRepository
                                        .RetrieveAvailableMetrics()
                                        .Select(m => m.ToViewModel())
                                        .OrderByDescending(cml => cml.Selected)
                                        .ThenBy(cml => cml.Name)
                                        .ToList()
            };

            return this.View(model);
        }
        
        [HttpPost]
        [Authorize(Roles = "AccountManager")]
        public ActionResult Create(CampaingViewModel campaingToCreate)
        {
            var datesValid = campaingToCreate.AreDatesValid();
            this.ClearInvalidErrors(campaingToCreate);

            if (this.ModelState.IsValid && datesValid)
            {
                var campaingId = this.campaingRepository.CreateCampaing(campaingToCreate.ToEntity(this.campaingRepository));
                this.campaingRepository.SaveCampaingMetricLevels(campaingToCreate.CampaingMetricLevels
                                                                                    .Where(cml => cml.Selected)
                                                                                    .Select(cml => cml.ToEntity(campaingId)));
                this.campaingRepository.SaveCampaingSupervisors(campaingToCreate.CampaingSupervisors
                                                                                    .Where(s => s.Selected)
                                                                                    .Select(s => s.ToEntity(campaingId, campaingToCreate.BeginDate, campaingToCreate.EndDate)));

                return this.RedirectToAction("Search", new { searchCriteria = campaingId, msg = Server.UrlEncode(string.Format(CultureInfo.InvariantCulture, "La nueva Campaña '{0}' se creó exitosamente.", campaingId)) });
            }

            if (!datesValid)
            {
                this.ModelState["EndDate"].Errors.Add("La fecha de inicio tiene que ser menor que la de fin.");
            }

            return this.View(campaingToCreate);
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult FindCustomer(string q)
        {
            var customers = this.campaingRepository
                                    .RetrieveCustomersByName(q)
                                    .Select(c => c.Name);

            // Returns raw text, one result on each line.
            return Content(string.Join("\n", customers));
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult AvailableSupervisores(int? campaingId, string beginDate, string endDate)
        {
            var supervisors = this.GetAvailableSupervisors(campaingId, beginDate, endDate);

            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { Supervisors = supervisors } };
        }

        [Authorize(Roles = "AccountManager")] 
        public ActionResult Edit(int campaingId)
        {
            var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
            if (campaing == null)
            {
                return this.View("NotFound", new CampaingDetailsViewModel());
            }

            var beginDate = campaing.BeginDate.ToString("dd/MM/yyyy", CultureInfo.CurrentUICulture);
            var endDate = campaing.EndDate.HasValue ? campaing.EndDate.Value.ToString("dd/MM/yyyy", CultureInfo.CurrentUICulture) : null;
            var model = new CampaingViewModel
            {
                Id = campaingId,
                IsEditing = true,
                BeginDate = beginDate,
                EndDate = endDate,
                Name = campaing.Name,
                Description = campaing.Description,
                CampaingType = campaing.CampaingType,
                CustomerName = campaing.Customer.Name,
                OptimalHourlyValue = campaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                ObjectiveHourlyValue = campaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                MinimumHourlyValue = campaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                CampaingSupervisors = this.GetAvailableSupervisors(campaingId, beginDate, endDate)
            };

            var availableMetrics = this.campaingRepository
                                            .RetrieveAvailableMetrics()
                                            .Select(m => m.ToViewModel())
                                            .ToList();
            var campaingMetricLevels = this.campaingRepository.RetrieveCampaingMetricLevels(campaingId);
            foreach (var campaingMetricLevel in campaingMetricLevels)
            {
                var metricLevel = availableMetrics.FirstOrDefault(cml => cml.Id == campaingMetricLevel.MetricId);

                metricLevel.Selected = true;
                metricLevel.OptimalLevel = campaingMetricLevel.OptimalLevel.ToString(CultureInfo.InvariantCulture);
                metricLevel.ObjectiveLevel = campaingMetricLevel.ObjectiveLevel.ToString(CultureInfo.InvariantCulture);
                metricLevel.MinimumLevel = campaingMetricLevel.MinimumLevel.ToString(CultureInfo.InvariantCulture);
            }

            model.CampaingMetricLevels = availableMetrics.OrderByDescending(cml => cml.Selected).ToList();

            return this.View("Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "AccountManager")]
        public ActionResult Edit(CampaingViewModel campaingToEdit)
        {
            var datesValid = campaingToEdit.AreDatesValid();
            this.ClearInvalidErrors(campaingToEdit);

            if (this.ModelState.IsValid && datesValid)
            {
                this.campaingRepository.EditCampaing(campaingToEdit.ToEntity(this.campaingRepository));
                this.campaingRepository.SaveCampaingMetricLevels(campaingToEdit.CampaingMetricLevels
                                                                                    .Where(cml => cml.Selected)
                                                                                    .Select(cml => cml.ToEntity(campaingToEdit.Id)));
                this.campaingRepository.UpdateCampaingSupervisors(campaingToEdit.CampaingSupervisors
                                                                                    .Where(s => s.Selected)
                                                                                    .Select(s => s.ToEntity(campaingToEdit.Id, campaingToEdit.BeginDate, campaingToEdit.EndDate)));

                return this.RedirectToAction("Search", new { searchCriteria = campaingToEdit.Id, msg = Server.UrlEncode(string.Format(CultureInfo.InvariantCulture, "La Campaña '{0}' se editó exitosamente.", campaingToEdit.Id)) });
            }

            if (!datesValid)
            {
                this.ModelState["EndDate"].Errors.Add("La fecha de inicio tiene que ser menor que la de fin.");
            }

            return this.View("Create", campaingToEdit);
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult End(int campaingId)
        {
            this.campaingRepository.EndCampaing(campaingId);

            return this.RedirectToAction("Search", new { searchCriteria = campaingId, msg = Server.UrlEncode(string.Format(CultureInfo.InvariantCulture, "La Campaña '{0}' se cerró exitosamente al día de hoy.", campaingId)) });
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

        private void ClearInvalidErrors(CampaingViewModel campaing)
        {
            for (var index = 0; index < campaing.CampaingMetricLevels.Count; index++)
            {
                if (!campaing.CampaingMetricLevels[index].Selected)
                {
                    var key = string.Format(CultureInfo.InvariantCulture, CampaingMetricLevelsKey, index, "OptimalLevel");
                    this.ModelState[key].Errors.Clear();

                    key = string.Format(CultureInfo.InvariantCulture, CampaingMetricLevelsKey, index, "ObjectiveLevel");
                    this.ModelState[key].Errors.Clear();

                    key = string.Format(CultureInfo.InvariantCulture, CampaingMetricLevelsKey, index, "MinimumLevel");
                    this.ModelState[key].Errors.Clear();
                }
            }
        }
        
        private IList<SupervisorViewModel> GetAvailableSupervisors(int? campaingId, string beginDate, string endDate)
        {
            var supervisors = new List<SupervisorViewModel>();
            DateTime begin;
            DateTime end;

            if (DateTime.TryParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out begin))
            {
                supervisors = DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end)
                    ? begin < end
                        ? this.campaingRepository
                            .RetrieveAvailableSupervisors(begin, end)
                            .Select(s => s.ToViewModel())
                            .ToList()
                        : supervisors
                    : this.campaingRepository
                            .RetrieveAvailableSupervisors(begin)
                            .Select(s => s.ToViewModel())
                            .ToList();

                if (campaingId.HasValue)
                {
                    supervisors.AddRange(this.membershipService
                                .RetrieveSupervisorsByCampaingId(campaingId.Value, int.MaxValue, 1)
                                .Select(s => s.ToViewModel(true)));
                }
            }

            return supervisors
                        .OrderByDescending(s => s.Selected)
                        .ToList();
        }

        private CampaingDetailsViewModel CreateCampaingDetailsViewModel(Campaing campaing, string searchCriteria, bool shouldPaginate, int page, int totalCount)
        {
            var availableMetricMonths = this.campaingRepository.RetrieveAvailableMonthsByCampaing(campaing.Id);

            var metricsDate = DateTime.Now.Date;
            var showEndCampaing = true;
            if (campaing.BeginDate.Date >= metricsDate)
            {
                metricsDate = campaing.BeginDate.Date;
                showEndCampaing = false;
            }
            else if (campaing.EndDate.HasValue && (campaing.EndDate.Value.Date <= metricsDate))
            {
                metricsDate = campaing.EndDate.Value.Date;
                showEndCampaing = false;
            }

            var model = new CampaingDetailsViewModel
            {
                CampaingId = campaing.Id,
                AvailableMetricMonths = availableMetricMonths,
                CurrentMetricMonthIndex = availableMetricMonths.IndexOf(metricsDate.ToString("yyyy-MM")),
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", campaing.Name, campaing.Id),
                Description = campaing.Description,
                Customer = campaing.Customer.Name,
                CampaingType = campaing.CampaingType == 0 ? "De Entrada" : "De Salida",
                AgentsCount = string.Format(CultureInfo.InvariantCulture, "{0} (ver todos)", this.campaingRepository.CountCampaingAgents(campaing.Id)),
                SupervisorsCount = string.Format(CultureInfo.InvariantCulture, "{0} (ver todos)", this.campaingRepository.CountCampaingSupervisors(campaing.Id)),
                BeginDate = campaing.BeginDate.ToString("dd/MM/yyyy", CultureInfo.CurrentUICulture),
                EndDate = campaing.EndDate.HasValue ? campaing.EndDate.Value.ToString("dd/MM/yyyy", CultureInfo.CurrentUICulture) : string.Empty,
                OptimalHourlyValue = campaing.OptimalHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                ObjectiveHourlyValue = campaing.ObjectiveHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                MinimumHourlyValue = campaing.MinimumHourlyValue.ToString("C", CultureInfo.CurrentUICulture),
                CurrentMetricLevel = this.CalculateMetricsLevel(campaing.Id, metricsDate, GetEndDate(campaing, metricsDate.Year, metricsDate.Month), true),
                ProjectedMetricLevel = this.CalculateMetricsLevel(campaing.Id, metricsDate, GetEndDate(campaing, metricsDate.Year, metricsDate.Month), false),
                MetricValues = this.CalculateCampaingMetricValues(campaing.Id, metricsDate),
                ShowEndCampaing = showEndCampaing,
                SearchCriteria = searchCriteria,
                ShouldPaginate = shouldPaginate,
                PageNumber = page,
                TotalPages = totalCount
            };

            return model;
        }

        private IList<MetricValuesViewModel> CalculateCampaingMetricValues(int campaingId, DateTime date)
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
                                CurrentValue = this.metricsRepository.GetCampaingMetricValue(campaingId, date, cml.MetricId).ToString("F", CultureInfo.InvariantCulture),
                                ProjectedValue = this.metricsRepository.GetCampaingMetricValue(campaingId, end, cml.MetricId).ToString("F", CultureInfo.InvariantCulture)
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
        
        private MetricLevel CalculateMetricsLevel(int campaingId, DateTime currentDate, DateTime projectedDate, bool currentMetricLevel)
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
                                                CurrentValue = this.metricsRepository.GetCampaingMetricValue(campaingId, currentDate, cml.MetricId),
                                                ProjectedValue = this.metricsRepository.GetCampaingMetricValue(campaingId, projectedDate, cml.MetricId)
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

        private Campaing GetCampaing(int? pageNumber, out int page, out int totalCount)
        {
            totalCount = this.campaingRepository.CountAllCampaings();

            if (totalCount == 0)
            {
                page = 1;
                return null;
            }

            page = !pageNumber.HasValue
                        ? 1
                        : pageNumber.Value > totalCount
                            ? totalCount
                            : pageNumber.Value;

            return this.campaingRepository.RetrieveAllCampaings(1, page).FirstOrDefault();
        }

        private Campaing SearchCampaing(string searchCriteria, int? pageNumber, out bool shoulPaginate, out int page, out int totalCount)
        {
            var campaingId = 0;
            if (int.TryParse(searchCriteria, NumberStyles.Integer, CultureInfo.InvariantCulture, out campaingId))
            {
                var campaing = this.campaingRepository.RetrieveCampaingById(campaingId);
                if (campaing != null)
                {
                    shoulPaginate = false;
                    page = 1;
                    totalCount = 1;

                    return campaing;
                }
            }

            var campaings = this.campaingRepository.SearchCampaings(searchCriteria);
            shoulPaginate = true;
            totalCount = campaings.Count;
            page = !pageNumber.HasValue
                        ? 1
                        : pageNumber.Value > totalCount
                            ? totalCount
                            : pageNumber.Value;


            return campaings.Skip(page - 1).Take(1).FirstOrDefault();
        }
    }
}
