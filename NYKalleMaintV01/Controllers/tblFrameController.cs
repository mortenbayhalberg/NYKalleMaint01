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
    public class tblFrameController : Controller
    {
        private NyKalleEntities db = new NyKalleEntities();

        // GET: tblFrame
        public ActionResult Index(int URLspillerId, int URLspilId)
        {

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId, URLspilId);

            List<tblFrame> ListFrame = db.tblFrames.Where(t => t.CN_FrameSpilPK == URLspilId).ToList();

            return View(ListFrame);
        }

        // GET: tblFrame/Details/5
        public ActionResult Details(int? id, int URLspillerId, int URLspilId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFrame tblFrame = db.tblFrames.Find(id);
            if (tblFrame == null)
            {
                return HttpNotFound();
            }

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId, URLspilId);

            return View(tblFrame);
        }

        // GET: tblFrame/Create
        public ActionResult Create(int URLspillerId, int URLspilId)
        {

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId, URLspilId);

            // Der skal ikke laves en drop down liste, der er valgt
            // ViewBag.CN_FrameSpilPK = new SelectList(db.tblSpils, "CN_SpilPK", "CN_SpilPK");

            return View();
        }

        // POST: tblFrame/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int URLspillerId, int URLspilId,[Bind(Include = "CN_FramePK,CN_FrameSpilPK,CN_FrameScore")] tblFrame tblFrame)
        {
            if (ModelState.IsValid)
            {

                tblFrame.CN_FrameSpilPK = URLspilId;

                db.tblFrames.Add(tblFrame);
                db.SaveChanges();
                return RedirectToAction("Index", new { URLspillerId, URLspilId });
            }

            ViewBag.CN_FrameSpilPK = new SelectList(db.tblSpils, "CN_SpilPK", "CN_SpilPK", tblFrame.CN_FrameSpilPK);
            return View(tblFrame);
        }

        // GET: tblFrame/Edit/5
        public ActionResult Edit(int? id, int URLspillerId, int URLspilId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFrame tblFrame = db.tblFrames.Find(id);
            if (tblFrame == null)
            {
                return HttpNotFound();
            }

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId, URLspilId);

            // Skal ikke bruges, der er valgt spil
            //ViewBag.CN_FrameSpilPK = new SelectList(db.tblSpils, "CN_SpilPK", "CN_SpilPK", tblFrame.CN_FrameSpilPK);

            return View(tblFrame);
        }

        // POST: tblFrame/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int URLspillerId, int URLspilId, [Bind(Include = "CN_FramePK,CN_FrameSpilPK,CN_FrameScore")] tblFrame tblFrame)
        {
            if (ModelState.IsValid)
            {

                tblFrame.CN_FrameSpilPK = URLspilId;

                db.Entry(tblFrame).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { URLspillerId, URLspilId });
            }
            ViewBag.CN_FrameSpilPK = new SelectList(db.tblSpils, "CN_SpilPK", "CN_SpilPK", tblFrame.CN_FrameSpilPK);
            return View(tblFrame);
        }

        // GET: tblFrame/Delete/5
        public ActionResult Delete(int? id, int URLspillerId, int URLspilId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFrame tblFrame = db.tblFrames.Find(id);
            if (tblFrame == null)
            {
                return HttpNotFound();
            }

            ViewBag.SpillerNavn = FnHentNavneOpl(URLspillerId, URLspilId);

            return View(tblFrame);
        }

        // POST: tblFrame/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int URLspillerId, int URLspilId)
        {
            tblFrame tblFrame = db.tblFrames.Find(id);
            db.tblFrames.Remove(tblFrame);
            db.SaveChanges();
            return RedirectToAction("Index", new { URLspillerId, URLspilId });
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
        private string FnHentNavneOpl(int URLspillerId, int URLspilId)
        {
            if (URLspillerId == 0)
            {
                return ("Du kalder med værdien: 0, derfor ikke noget navn til dig basta!!!.");
            }
            if (URLspilId == 0)
            {
                return ("Du kalder med værdien: 0, derfor ikke noget spil til dig bum!!!.");
            }

            ViewBag.URLspillerId = URLspillerId;
            ViewBag.URLspilId = URLspilId;

            var spillerInfo = db.tblSpillers.FirstOrDefault(t => t.CN_SpillerPK == URLspillerId);
            return (spillerInfo.CN_SpillerForNavn + " " + spillerInfo.CN_SpillerEfterNavn);
        }
        
        // Nu skal der beregnes gennemsnit af dagen for spillene (Frame)
        public ActionResult FnBeregnGennemsnit(int URLspillerId, int URLspilId)
        {
            //Først skal der opsummeres:

            // Først lidt housekeeping.
            decimal Gennemsnit = 0;
            int AntFrames = 0;

            //Hent de Frames der er tale om.
            List<tblFrame> ListFrame = db.tblFrames.Where(t => t.CN_FrameSpilPK == URLspilId).ToList();

            //Så skal der lige løbes igennem og findes gennemsnit:
            foreach (var item in ListFrame)
            {
                Gennemsnit = Gennemsnit + item.CN_FrameScore;
                AntFrames = AntFrames + 1;
            }
            //Så skal der lige regnes lidt på det.
            if (AntFrames > 0)
            {
                Gennemsnit = Gennemsnit / AntFrames;
            }

            //Klar til opdatering af tblSpil
            tblSpil tblSpil = db.tblSpils.Find(URLspilId);

            if (tblSpil != null)
            {
                tblSpil.CN_SpilGennemsnit = Gennemsnit;

                db.Entry(tblSpil).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "tblSpil", new { URLspillerId });
         }
    }
}
