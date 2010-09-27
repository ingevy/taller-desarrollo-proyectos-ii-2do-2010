namespace CallCenter.SelfManagement.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;
    using System.Globalization;
    
    public class CampaingController : Controller
    {
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
            return View();
        }

        //
        // GET: /Campaing/Create
        [Authorize(Roles = "AccountManager")]
        public ActionResult Create()
        {
            var model = new CampaingViewModel
            {
                BeginDate = DateTime.Today.ToShortDateString(),
                Supervisors = this.campaingRepository.RetrieveAvailableSupervisors(DateTime.Today)
                                .Select(up => new SupervisorViewModel { Id = up.InnerUserId, DisplayName = GetDisplayName(up) })
                                .ToList(),
                Metrics = this.campaingRepository
                                .RetrieveAvailableMetrics()
                                .Select(m => new MetricViewModel { Id = m.Id, Name = m.MetricName, Description = m.ShortDescription, FormatType = m.Format })
                                .ToList()
            };

            return View("Create", model);
        }

        //
        // POST: /Campaing/Create
        [HttpPost]
        [Authorize(Roles = "AccountManager")]
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

        // The autocomplete request sends a parameter 'q' that contains the filter
        [Authorize(Roles = "AccountManager")]
        public ActionResult Find(string q)
        {
            var customers = this.campaingRepository
                                    .SearchCustomer(q)
                                    .Select(c => c.Name);

            // Returns raw text, one result on each line.
            return Content(string.Join("\n", customers));
        }

        [Authorize(Roles = "AccountManager")]
        public ActionResult AvailableSupervisores(DateTime beginDate, DateTime? endDate)
        {
            var supervisors = this.campaingRepository
                                .RetrieveAvailableSupervisors(beginDate, endDate)
                                .Select(s => GetDisplayName(s));

            // Returns raw text, one result on each line.
            return Content(string.Join("\n", supervisors));
        }

        //
        // GET: /Campaing/Edit/5
        [Authorize(Roles = "AccountManager")] 
        public ActionResult Edit(int id)
        {
            return View();
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
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Campaing/Delete/5
        [Authorize(Roles = "AccountManager")] 
        public ActionResult Delete(int id)
        {
            return View();
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
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private static string GetDisplayName(UserProfile userProfile)
        {
            if (!string.IsNullOrEmpty(userProfile.Name) && !string.IsNullOrEmpty(userProfile.LastName))
            {
                return string.Format(CultureInfo.CurrentUICulture, "{0} {1} ({2})", userProfile.Name, userProfile.LastName, userProfile.InnerUserId);
            }

            return string.Format(CultureInfo.CurrentUICulture, "{0} ({1})", userProfile.UserName, userProfile.InnerUserId);
        } 
    }
}
