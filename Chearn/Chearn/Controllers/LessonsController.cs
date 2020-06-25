using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Chearn.Models;
using Microsoft.AspNet.Identity;
using Chearn.Models.dtos;
namespace Chearn.Controllers
{
    public class LessonsController : Controller
    {
        private ChearnContext db = new ChearnContext();
       
        private static int ActiveCourseID = -1;

        public JsonResult GetAllLessons(int courseID)
        {
            var course = db.Courses.Find(courseID);
            if (course != null && course.Instructor.CUser.AspID == User.Identity.GetUserId())
            {
                return Json(db.Lessons.Where(l => l.CourseID == courseID).Select(x => new { x.ID, x.Title }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStudentLessons(int courseID) {
            var course = db.Courses.Find(courseID);
            if (course != null)
            {
                var edgesCollections = db.Lessons.Where(l =>
                    l.CourseID == courseID && l.Edges.Any()).Select(x => x.Edges).ToArray();

                var linkedNodes = new LinkedList<int>();
                foreach (var edges in edgesCollections)
                {
                    foreach (var edge in edges)
                    {
                        if (edge.ChildID.HasValue && !linkedNodes.Contains(edge.ChildID.Value))
                            linkedNodes.AddLast(edge.ChildID.Value);
                        if (edge.ParentID.HasValue && !linkedNodes.Contains(edge.ParentID.Value))
                            linkedNodes.AddLast(edge.ParentID.Value);
                    }
                }
                var lessons = db.Lessons.Where(l =>
                    linkedNodes.Contains(l.ID)).Select(l => new { l.ID, l.Title }).ToArray();
                
                return Json(lessons, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult GetAllConnectedLessons(int courseID)
        {
            var course = db.Courses.Find(courseID);
            if (course != null && course.Instructor.CUser.AspID == User.Identity.GetUserId())
            {
                var edgesCollections = db.Lessons.Where(l =>
                    l.CourseID == courseID && l.Edges.Any()).Select(x => x.Edges ).ToArray();

                var linkedNodes = new LinkedList<int>();
                foreach(var edges in edgesCollections)
                {
                    foreach(var edge in edges)
                    {
                        if (edge.ChildID.HasValue && !linkedNodes.Contains(edge.ChildID.Value))
                            linkedNodes.AddLast(edge.ChildID.Value);
                        if (edge.ParentID.HasValue && !linkedNodes.Contains(edge.ParentID.Value))
                            linkedNodes.AddLast(edge.ParentID.Value);   
                    }
                }
                var lessons = db.Lessons.Where(l =>
                    linkedNodes.Contains(l.ID)).Select(l => new { l.ID, l.Title } ).ToArray();
                //var lessons = db.Lessons.Where(l =>
                  //  l.CourseID == courseID && l.Edges.Contains(edgeIDs)).Select(x => new { x.ID, x.Title }).ToArray();
                return Json(lessons, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        //GET
        public JsonResult GetLessonsByName(int courseID, string name)
        {
            var course = db.Courses.Find(courseID);
           
                return  Json(db.Lessons.Where(l => l.CourseID == courseID
                && l.Title.Contains(name)).ToList().Select(x => new { x.ID, x.Title, Description = x.MaterialA.Count() > 150 ? x.MaterialA.Substring(0, 150) : x.MaterialA }), JsonRequestBehavior.AllowGet);


        }

       

        [Authorize(Roles ="Instructor")]
        // GET: Lessons
        public ActionResult Index( )
        {
            //User ID of Logged in user
            var userID = User.Identity.GetUserId();
            //Course 
            //var course = db.Courses.Find(courseID);
            //Lessons
            var lessons = db.Lessons.Include(l => l.Cours);
            
            //checks whether anyone is signed in
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "The user is not currently signed in.");

            }

            //checks whether current is an instructor
           
           
            
            return View(lessons.ToList());
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create 


        //Edit Linq to make it return just the Courses that belong to the instructor that is logged in
        public ActionResult Create(int? id)
        {
            LessonsController.ActiveCourseID = id ?? -1;

            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "The user is not currently signed in.");

            }

            //checks whether current is an instructor
            var userID = User.Identity.GetUserId();

            if (!InstructorController.UserIsInstructor(userID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Only instructors may access this page.");
            }

            ViewBag.CourseID = id;
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id,[Bind(Include = "ID,Title,MaterialA,MaterialB,ImageLink,VideoLink,CourseID")] Lesson lesson)
        {
            var course = db.Courses.Single(X => X.ID == id);

            if (ModelState.IsValid)
            {
                if (LessonsController.ActiveCourseID != -1)
                    lesson.CourseID = LessonsController.ActiveCourseID;
                db.Lessons.Add(lesson);
                _ = db.SaveChanges();
                return RedirectToAction("Edit","cours",course);
            }

            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", lesson.CourseID);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", lesson.CourseID);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "ID,Title,MaterialA,MaterialB,ImageLink,VideoLink,CourseID")] Lesson lesson)
        {
            var course = db.Courses.Single(X => X.ID == id);
            if (ModelState.IsValid)
            {
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit","cours",course);
            }
            ViewBag.CourseID = new SelectList(db.Courses, "ID", "Name", lesson.CourseID);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("Index","cours");
        }

        public bool IsAutheticated(int? id, out ActionResult errorResult)
        {
            errorResult = null;
            //checks whether ID was passed in
            if (id == null)
            {
                errorResult = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var course = db.Courses.Find(id.Value);
            //checks whether course exists
            if (course == null)
            {
                errorResult = HttpNotFound();
            }
            //checks whether anyone is signed in
            if (!User.Identity.IsAuthenticated)
            {
                errorResult = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "The user is not currently signed in.");
            }
            //checks whether current is an instructor
            var userID = User.Identity.GetUserId();
            if (!InstructorController.UserIsInstructor(userID))
            {
                errorResult = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Only instructors may access this page.");
            }

            var instructor = db.Instructors.Where(i =>
                i.CUserID == db.CUsers.Where(c =>
                    c.AspID == userID).FirstOrDefault().ID).SingleOrDefault();
            if (instructor.ID != course.InstructorID)
            {
                errorResult = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "This is not your course to edit");
            }
            if (errorResult != null)
                return false;

            return true;
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
