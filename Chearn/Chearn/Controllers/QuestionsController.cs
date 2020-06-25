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
    public class QuestionsController : Controller
    {
        private ChearnContext db = new ChearnContext();
        
        private static int ActiveLessonID = -1;

        [Authorize(Roles = "Instructor")]
        //GetAll?ID=${data.getAttribute("val")
        ///SetPoints/?ID=${questions.ID}&Points=${points.value}`
        public void SetPoints(int ID, int Points)
        {
            db.Questions.Find(ID).Points = Points;
            db.SaveChanges();
        }
        public ActionResult Help()
        {
            return View();
        }
        public JsonResult GetAll(int ID)
        {
            var lessons = db.Lessons.Where(l => l.CourseID == ID).Select(x => x.ID).ToList();
            var questions = db.Questions.Where(q => lessons.Contains(q.LessonID.Value))
                .Select(x => new { x.ID, x.Text, x.Points}).ToList();
            return Json(questions, JsonRequestBehavior.AllowGet);

        }
        // GET: Questions
        public ActionResult Index(int? id)
        {
            ViewBag.CourseID = id.Value;
            var questions = db.Questions.Include(q => q.Lesson);
            return View(questions.ToList());
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        [Authorize(Roles = "Instructor")]

        // GET: Questions/Create
        public ActionResult Create(int? id)
        {
           var lesson = db.Lessons.Single(X => X.ID == id);
            ViewBag.Lesson = lesson;

             //ViewBag.LessonID = new SelectList(db.Lessons, "ID", "Title");
            return View();
        }


        [Authorize(Roles = "Instructor")]

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonID,Text,Points,Option1,Option2,Option3,Option4,Answer")] Question question)
        {
            //var course = db.Courses.Single(X => X.ID == id);
           // var lessonID = db.Lessons.Single(x => x.ID == id);
            if (ModelState.IsValid)
            {
                if (QuestionsController.ActiveLessonID != -1)
                    question.LessonID = QuestionsController.ActiveLessonID;
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index","cours");
            }

            //ViewBag.LessonID = new SelectList(db.Lessons, "ID", "Title", question.LessonID);
            return View(question);
        }

        [Authorize(Roles = "Instructor")]

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.LessonID = new SelectList(db.Lessons, "ID", "Title", question.LessonID);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LessonID,Text,Option1,Option2,Option3,Option4,Answer")] Question question)
        {
           // var course = db.Courses.Single(X => X.ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","cours");
            }
            ViewBag.LessonID = new SelectList(db.Lessons, "ID", "Title", question.LessonID);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
