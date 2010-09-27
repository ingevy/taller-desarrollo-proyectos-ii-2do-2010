using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CallCenter.SelfManagement.Web.Controllers
{
    public class SupervisorController : Controller
    {
        //
        // GET: /Supervisor/
        [Authorize(Roles = "AccountManager, Supervisor")]
        public ActionResult Index()
        {
            return View();
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
    }
}
