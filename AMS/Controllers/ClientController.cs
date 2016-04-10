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
	public class ClientController : Controller
    {
        private AMSContext db = new AMSContext();

        // GET: Client
        public ActionResult Index()
        {
            return View(Mapper.Map<List<ClientViewModel>>(db.Clients.ToList()));
        }

        // GET: Client/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

	        var x = db.Clients.Find(id);

	        ClientViewModel clientViewModel = Mapper.Map<ClientViewModel>(db.Clients.Find(id));
            if (clientViewModel == null)
            {
                return HttpNotFound();
            }
            return View(clientViewModel);
        }

        // GET: Client/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ClientCode")] ClientViewModel clientViewModel)
        {
            if (ModelState.IsValid)
            {
	            Client client = Mapper.Map<Client>(clientViewModel);

				db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientViewModel);
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientViewModel clientViewModel = Mapper.Map<ClientViewModel>(db.Clients.Find(id));
			if (clientViewModel == null)
            {
                return HttpNotFound();
            }
            return View(clientViewModel);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ClientCode")] ClientViewModel clientViewModel)
        {
            if (ModelState.IsValid)
            {
	            Client client = db.Clients.Find(clientViewModel.ID);
	            client.Name = clientViewModel.Name;
	            client.ClientCode = clientViewModel.ClientCode;
				db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientViewModel);
        }

        // GET: Client/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientViewModel clientViewModel = Mapper.Map<ClientViewModel>(db.Clients.Find(id));
            if (clientViewModel == null)
            {
                return HttpNotFound();
            }
            return View(clientViewModel);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			Client client = db.Clients.Find(id);
			db.Clients.Remove(client);
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
