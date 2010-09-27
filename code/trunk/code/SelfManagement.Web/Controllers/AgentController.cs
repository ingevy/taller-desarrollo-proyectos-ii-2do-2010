namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;

    public class AgentController : Controller
    {
        private readonly ICampaingRepository campaingRepository;

        public AgentController() : this(new RepositoryFactory().GetCampaingRepository())
        {
        }

        public AgentController(ICampaingRepository campaingRepository)
        {
            this.campaingRepository = campaingRepository;
        }

        //
        // GET: /Agent/
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Agent/Create
        [Authorize(Roles = "AccountManager, Supervisor, Agent")]
        public ActionResult Create()
        {
            return View();
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

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Agent/Edit/5
        [Authorize(Roles = "AccountManager, Supervisor, Agent")] 
        public ActionResult Edit(int id)
        {
            return View();
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
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Agent/Delete/5
        [Authorize(Roles = "AccountManager, Supervisor, Agent")] 
        public ActionResult Delete(int id)
        {
            return View();
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
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
