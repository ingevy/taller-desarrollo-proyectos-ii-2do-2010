﻿namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;

    public class CampaingController : Controller
    {
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
            return View();
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
    }
}
