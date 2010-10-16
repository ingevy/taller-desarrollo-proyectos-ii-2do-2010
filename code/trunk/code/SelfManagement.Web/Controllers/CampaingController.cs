namespace CallCenter.SelfManagement.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;
    
    public class CampaingController : Controller
    {
        private const string CampaingMetricLevelsKey = "CampaingMetricLevels[{0}].{1}";
        private readonly ICampaingRepository campaingRepository;

        public CampaingController() : this(new RepositoryFactory().GetCampaingRepository())
        {
        }

        public CampaingController(ICampaingRepository campaingRepository)
        {
            this.campaingRepository = campaingRepository;
        }

        //
        // GET: /Campaing/
        [Authorize(Roles = "AccountManager")]
        public ActionResult Index()
        {
            return this.View();
        }

        //
        // GET: /Campaing/Create
        [Authorize(Roles = "AccountManager")]
        public ActionResult Create()
        {
            var model = new CampaingViewModel
            {
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

        //
        // POST: /Campaing/Create
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
                

                return this.RedirectToAction("Index", new { msg = Server.UrlEncode("La nueva campaña se creó exitosamente.")});
            }

            //campaingToCreate.CampaingSupervisors = campaingToCreate.CampaingSupervisors
            //                                                        .OrderByDescending(s => s.Selected)
            //                                                        .ToList();

            //campaingToCreate.CampaingMetricLevels = campaingToCreate.CampaingMetricLevels
            //                                                        .OrderByDescending(cml => cml.Selected)
            //                                                        .ToList();

            if (!datesValid)
            {
                this.ModelState["EndDate"].Errors.Add("La fecha de inicio tiene que ser menor que la de fin.");
            }

            return this.View(campaingToCreate);
        }

        // The autocomplete request sends a parameter 'q' that contains the filter
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
        public ActionResult AvailableSupervisores(string beginDate, string endDate)
        {
            IEnumerable<SupervisorViewModel> supervisors = new List<SupervisorViewModel>();
            DateTime begin;
            DateTime end;

            if (DateTime.TryParseExact(beginDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out begin))
            {
                supervisors = DateTime.TryParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end)
                    ? begin < end
                        ? this.campaingRepository
                            .RetrieveAvailableSupervisors(begin, end)
                            .Select(s => s.ToViewModel())
                        : supervisors
                    : this.campaingRepository
                            .RetrieveAvailableSupervisors(begin)
                            .Select(s => s.ToViewModel());                        
            }

            // Returns raw text, one result on each line.
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { Supervisors = supervisors } };
        }

        //
        // GET: /Campaing/Edit/5
        [Authorize(Roles = "AccountManager")] 
        public ActionResult Edit(int id)
        {
            return this.View();
        }

        //
        // POST: /Campaing/Edit/5
        [HttpPost]
        [Authorize(Roles = "AccountManager")]
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
        // GET: /Campaing/Delete/5
        [Authorize(Roles = "AccountManager")] 
        public ActionResult Delete(int id)
        {
            return this.View();
        }

        //
        // POST: /Campaing/Delete/5
        [HttpPost]
        [Authorize(Roles = "AccountManager")]
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
    }
}
