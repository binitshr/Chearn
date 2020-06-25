using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Chearn.Models;
using Microsoft.AspNet.Identity;
using Chearn.Models.ViewModels;

namespace Chearn.Controllers
{
    public class StudentsController : Controller
    {
        private ChearnContext db = new ChearnContext();
        public static Student GetCurrentStudent(string id)
        {
            var db = new ChearnContext();
           
            return db.Students.Where(s
                => s.CUserID == db.CUsers.Where(c
                    => c.AspID == id).FirstOrDefault().ID).FirstOrDefault();
        }

        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.CUser);
            return View(students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details()
        {
            Student student = null;
            if (User.Identity.IsAuthenticated && GetActiveStudent() != null)
            {
                student = GetActiveStudent();
            }
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.CUserID = new SelectList(db.CUsers, "ID", "Email");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,YearsOfEducation,CUserID")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CUserID = new SelectList(db.CUsers, "ID", "Email", student.CUserID);
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            var editStudentViewModel = new EditStudentViewModel
            {
                ID = student.ID,
                CUserID = student.CUserID.Value,
                About = student.CUser.About,
                YearsOfEducation = student.YearsOfEducation,
                Email = student.CUser.Email,
                FirstName = student.CUser.FirstName,
                LastName = student.CUser.LastName
            };
            return View(editStudentViewModel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditStudentViewModel editStudentViewModel)
        {
            if (ModelState.IsValid)
            {
                var oldStudent = db.Students.Find(editStudentViewModel.ID);
                var oldUser = db.CUsers.Find(oldStudent.CUserID);
                var updatedStudent = new Student
                {
                    ID = oldStudent.ID,
                    CUserID = oldStudent.ID,
                    CUser = oldStudent.CUser,
                    StudentLessons = oldStudent.StudentLessons,
                    YearsOfEducation = editStudentViewModel.YearsOfEducation
                };

                oldUser.About = editStudentViewModel.About;
                oldUser.Email = editStudentViewModel.Email;
                oldUser.FirstName = editStudentViewModel.FirstName;
                oldUser.LastName = editStudentViewModel.LastName;

                db.Students.Remove(oldStudent);
                db.SaveChanges();
                db.Entry(oldUser).State = EntityState.Modified;
                db.SaveChanges();

                db.Students.Add(updatedStudent);
                db.SaveChanges();
                return RedirectToAction("Details", "Students");
            }
            return View(editStudentViewModel);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //gets the current active user if exists
        private Student GetActiveStudent()
        {
            var id = User.Identity.GetUserId();
            return db.Students.Where(s 
                => s.CUserID == db.CUsers.Where(c 
                    => c.AspID == id).FirstOrDefault().ID).FirstOrDefault();
        }

        public static bool UserIsStudent(string id) 
        {
            var db = new ChearnContext();
            return db.Students.Where(s
                => s.CUserID == db.CUsers.Where(c
                    => c.AspID == id).FirstOrDefault().ID).FirstOrDefault() != null;
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
