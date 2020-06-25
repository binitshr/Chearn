using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chearn.Models;

namespace Chearn.Controllers
{
    public class CoursesAvailableController : Controller
    {
        private ChearnContext db = new ChearnContext();

        // GET: CoursesAvailable
        public ActionResult Index()
        {
            var courses = db.Courses.Where(c => c.Comfirmed == true);
            return View(courses.ToList());
        }

        [HttpPost]
        public ActionResult Index(string Search)
        {
            if (Search == null)
            {
                return View();
            }

            var tagMatches = db.Courses.Where(x => x.Tag.Contains(Search) && x.Comfirmed == true).ToList();
            var nameMatches = db.Courses.Where(x => x.Name.Contains(Search) && x.Comfirmed == true).ToList();
            if (nameMatches != null) {
                for (int i = 0; i != nameMatches.Count; i++)
                {
                    tagMatches.Add(nameMatches[i]);
                }
            }
            return View(tagMatches);
        }

        public ActionResult ImageShow(int id)
        {
            var course = db.Courses.Find(id);
            return File(course.Picture, "image/jpg");
        }

        // GET: CoursesAvailable/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // GET: CoursesAvailable/Create
        public ActionResult Create()
        {
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "ID");
            return View();
        }

        // POST: CoursesAvailable/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InstructorID,Name")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "ID", cours.InstructorID);
            return View(cours);
        }

        // GET: CoursesAvailable/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "ID", cours.InstructorID);
            return View(cours);
        }

        // POST: CoursesAvailable/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,InstructorID,Name")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "ID", cours.InstructorID);
            return View(cours);
        }

        // GET: CoursesAvailable/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                return HttpNotFound();
            }
            return View(cours);
        }

        // POST: CoursesAvailable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cours cours = db.Courses.Find(id);
            db.Courses.Remove(cours);
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
