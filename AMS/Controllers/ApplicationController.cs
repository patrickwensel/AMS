using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;
using AMS.ViewModels;
using AutoMapper;

namespace AMS.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ApplicationController : Controller
    {
        private AMSContext db = new AMSContext();

        // GET: Application
        public ActionResult Index()
        {
			return View(Mapper.Map<List<ApplicationViewModel>>(db.Applications.ToList()));
			//return View(db.ApplicationViewModels.ToList());
		}

        // GET: Application/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			ApplicationViewModel applicationViewModel = Mapper.Map<ApplicationViewModel>(db.Applications.Find(id));
			if (applicationViewModel == null)
            {
                return HttpNotFound();
            }
            return View(applicationViewModel);
        }

        // GET: Application/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Application/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] ApplicationViewModel applicationViewModel)
        {
            if (ModelState.IsValid)
            {
				Application application = Mapper.Map<Application>(applicationViewModel);
				db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationViewModel);
        }

        // GET: Application/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			ApplicationViewModel applicationViewModel = Mapper.Map<ApplicationViewModel>(db.Applications.Find(id));
			
            if (applicationViewModel == null)
            {
                return HttpNotFound();
            }
            return View(applicationViewModel);
        }

        // POST: Application/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] ApplicationViewModel applicationViewModel)
        {
            if (ModelState.IsValid)
            {
				Application application = db.Applications.Find(applicationViewModel.ID);
				application.Name = applicationViewModel.Name;
				db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationViewModel);
        }

        // GET: Application/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			ApplicationViewModel applicationViewModel = Mapper.Map<ApplicationViewModel>(db.Applications.Find(id));
			
            if (applicationViewModel == null)
            {
                return HttpNotFound();
            }
            return View(applicationViewModel);
        }

        // POST: Application/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			Application application = db.Applications.Find(id);
			db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
