using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NYKalleMaintV01.Models;

namespace NYKalleMaintV01.Controllers
{
    public class tblSpilController : Controller
    {
        private NyKalleEntities db = new NyKalleEntities();

        // GET: tblSpil
        public ActionResult Index(int URLspillerId)
        {
            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId);

            List<tblSpil> ListSpil = db.tblSpils.OrderByDescending(t => t.CN_SpilDato).Where(t => t.CN_SpilSpillerPK == URLspillerId).ToList();
            return View(ListSpil);
        }

        // GET: tblSpil/Details/5
        public ActionResult Details(int? id, int URLspillerId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSpil tblSpil = db.tblSpils.Find(id);
            if (tblSpil == null)
            {
                return HttpNotFound();
            }
            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId);

            return View(tblSpil);
        }

        // GET: tblSpil/Create
        public ActionResult Create(int URLspillerId)
        {
            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId);

            // Skal ikkebenytte en list, Spilleren er udpeget og kun de spil som spilleren har skal listes
            // ViewBag.CN_SpilSpillerPK = new SelectList(db.tblSpillers, "CN_SpillerPK", "CN_SpillerForNavn");

            return View();
        }

        // POST: tblSpil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int URLspillerId, [Bind(Include = "CN_SpilPK,CN_SpilSpillerPK,CN_SpilDato,CN_SpilSesson,CN_SpilKalleKamel,CN_SpilPlacering,CN_SpilGennemsnit")] tblSpil tblSpil)
        {
            if (ModelState.IsValid)
            {
                tblSpil.CN_SpilSpillerPK = URLspillerId;

                db.tblSpils.Add(tblSpil);
                db.SaveChanges();
                return RedirectToAction("Index", new { URLspillerId = tblSpil.CN_SpilSpillerPK });
            }

            // Spilleren er allerede valgt
            // ViewBag.CN_SpilSpillerPK = new SelectList(db.tblSpillers, "CN_SpillerPK", "CN_SpillerForNavn", tblSpil.CN_SpilSpillerPK);

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId);

            return View(tblSpil);
        }

        // GET: tblSpil/Edit/5
        public ActionResult Edit(int? id, int URLspillerId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSpil tblSpil = db.tblSpils.Find(id);
            if (tblSpil == null)
            {
                return HttpNotFound();
            }

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId);

            // Skal ikke bruges. Spiller er valgt
            // ViewBag.CN_SpilSpillerPK = new SelectList(db.tblSpillers, "CN_SpillerPK", "CN_SpillerForNavn", tblSpil.CN_SpilSpillerPK);

            return View(tblSpil);
        }

        // POST: tblSpil/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int URLspillerId, [Bind(Include = "CN_SpilPK,CN_SpilSpillerPK,CN_SpilDato,CN_SpilSesson,CN_SpilKalleKamel,CN_SpilPlacering,CN_SpilGennemsnit")] tblSpil tblSpil)
        {
            if (ModelState.IsValid)
            {
                tblSpil.CN_SpilSpillerPK = URLspillerId;

                db.Entry(tblSpil).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", new { URLspillerId });
            }
            ViewBag.CN_SpilSpillerPK = new SelectList(db.tblSpillers, "CN_SpillerPK", "CN_SpillerForNavn", tblSpil.CN_SpilSpillerPK);
            return View(tblSpil);
        }

        // GET: tblSpil/Delete/5
        public ActionResult Delete(int? id, int URLspillerId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSpil tblSpil = db.tblSpils.Find(id);
            if (tblSpil == null)
            {
                return HttpNotFound();
            }

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId);

            return View(tblSpil);
        }

        // POST: tblSpil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int URLspillerId)
        {
            tblSpil tblSpil = db.tblSpils.Find(id);
            db.tblSpils.Remove(tblSpil);
            db.SaveChanges();
            return RedirectToAction("Index", new { URLspillerId });
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        // Hent Fornavn og Efternavn på spiller
        private string FnHentNavneOpl(int URLspillerId)
        {
            if (URLspillerId == 0)
            {
                return ("Du kalder med værdien: 0, derfor ikke noget navn til dig basta.");
            }

            ViewBag.URLspillerId = URLspillerId;

            var spillerInfo = db.tblSpillers.FirstOrDefault(t => t.CN_SpillerPK == URLspillerId);
            return (spillerInfo.CN_SpillerForNavn + " " + spillerInfo.CN_SpillerEfterNavn);
        }
    }
}
