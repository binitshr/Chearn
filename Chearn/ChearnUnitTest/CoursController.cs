using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Chearn.Models;
using Chearn.Models.ViewModels;
using Microsoft.AspNet.Identity;


namespace Chearn.Controllers
{
    
    public class CoursController : Controller
    {
        private ChearnContext db = new ChearnContext();

        [Authorize(Roles = "Instructor")]
        // GET: Cours
        public ActionResult Index(  )
        {
            //var viewModel = new courseIndexData();
            //viewModel.Courses = db.Courses
            // .Include(i => i.Lessons.Select(c => c.CourseID))
            //.OrderBy(i => i.InstructorID);

          
            //checks whether current is an instructor
            var aspID = User.Identity.GetUserId();
 
            var cuser = db.CUsers.Single(i => i.AspID == aspID);
            
            var courses = db.Courses.Where(c => c.InstructorID == cuser.ID);

            return View(courses.ToList());
        }


        [Authorize(Roles = "Instructor")]
        // GET: Cours/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InstructorID,Name")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                cours.InstructorID = InstructorController.GetCurrentInstructor(User.Identity.GetUserId()).ID;
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cours);
        }

        [Authorize(Roles = "Instructor")]
        // GET: Cours/Edit/5
        public ActionResult Edit(int? id)
        {
            ActionResult errorResult;
            var isAutheticated = this.IsAutheticated(id, out errorResult);
            if (!isAutheticated)
                return errorResult;

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

        [Authorize(Roles = "Instructor")]
        // POST: Cours/Edit/5
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

        [Authorize(Roles = "Instructor")]
        // GET: Cours/Delete/5
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

        [Authorize(Roles = "Instructor")]
        // POST: Cours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActionResult result;
            var isAuthenticated = this.IsAutheticated(id, out result);
            if (!isAuthenticated)
                return result;

            var course = db.Courses.Find(id);
            var lessons = course.Lessons;
            db.Lessons.RemoveRange(lessons);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");

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

        [Authorize(Roles = "Instructor")]
        public ActionResult NodeGraph(int? id)
        {
            ViewBag.CourseID = id.Value;
            ActionResult errorResult;
            var isAutheticated = this.IsAutheticated(id, out errorResult);
            if (!isAutheticated)
                return errorResult;


            var course = db.Courses.Find(id);
            return View(course);
        }

        

        public ActionResult StudentCourses()
        {
            var aspID = User.Identity.GetUserId();

            var cuser = db.CUsers.Single(i => i.AspID == aspID);

            var courses = db.Enrollments.Where(c => c.StudentID == cuser.ID);

            return View(courses.ToList());
        }

    

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        [Authorize(Roles = "Student")]
        // GET: Cours/Details/5
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

    }
   
}

