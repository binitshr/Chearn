using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Chearn.Models;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Web.Security;

namespace Chearn.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ChearnContext db = new ChearnContext();

        public ActionResult AllUsers()
        {
            return View(db.CUsers.ToList());
        }

        public ActionResult Deactivate(int id)
        {
            CUser user = db.CUsers.Find(id);
            if (Roles.IsUserInRole(user.Email, "Instructor"))
            {
                Roles.RemoveUserFromRole(user.Email, "Instructor");
                Roles.AddUserToRole(user.Email, "Undecided");
            }else if (Roles.IsUserInRole(user.Email, "Instructor"))
            {
                Roles.RemoveUserFromRole(user.Email, "Student");
                Roles.AddUserToRole(user.Email, "Undecided");
            }
            return RedirectToAction("AllUsers");
        }


        // GET: Admin
        public ActionResult AllCourses()
        {
            return View(db.Courses.ToList());
        }

        public ActionResult Roster(int? id)
        {
            var List = db.Enrollments.Where(x => x.CourseID == id).ToList();
            List<CUser> Students = new List<CUser>();
            foreach (var item in List)
            {
                var temp = db.Students.Find(item.StudentID);
                Students.Add(db.CUsers.Find(temp.CUserID));
            }
            return View(Students);
        }

        public ActionResult ValidationList()
        {
            var NoLessons = db.Courses.Where(x => x.Valid == true && x.Lessons.Count == 0).ToList();
            if (NoLessons.Count != 0)
            {
                foreach (var shell in NoLessons)
                {
                    shell.Valid = false;
                }
            }
            return View(db.Courses.Where(x => x.Valid == true && x.Comfirmed == false && x.Lessons.Count != 0).ToList());
        }
        [HttpGet]
        public ActionResult ListOfLessons(int? id)
        {
            var lessons = db.Lessons.Where(x => x.CourseID == id);
            return View(lessons);
        }

        [HttpPost]
        public ActionResult ListOfLessons(int id, string Accept, string Deny)
        {
            var course = db.Courses.Find(id);
            if (Accept == "Accept")
            {
                course.Comfirmed = true;
            }
            else
            {
                course.Valid = false;
            }
            db.SaveChanges();
            return RedirectToAction("ValidationList");
        }
    }
}