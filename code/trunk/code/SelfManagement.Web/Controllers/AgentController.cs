namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;
    using System.Globalization;

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
                // Update
                Salary = new SalaryViewModel { GrossSalary = 2000, VariableSalary = 554.5, TotalSalary = 2554.5 },
                AvailableMonths = new List<string> { "2010-07", "2010-08", "2010-09", "2010-10" },
                // Update
                DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", agent.Name, agent.LastName, agent.InnerUserId),
                CurrentSupervisor = string.Format(CultureInfo.InvariantCulture, "{0} {1} ({2})", currentSupervisor.Name, currentSupervisor.LastName, currentSupervisor.InnerUserId),
                CurrentCampaingId = currentCampaing != null ? currentCampaing.Id : 0,
                AgentCampaings = this.campaingRepository
                                        .RetrieveCampaingsByUserId(agent.InnerUserId)
                                        .Select(c => c.ToUserCampaingInfo())
                                        .ToList()
            };

            return this.View(model);
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
    }
}
