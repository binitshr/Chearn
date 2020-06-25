using Chearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chearn.Controllers
{
    public class InstructorController : Controller
    {
        // GET: Instructor
        public ActionResult Index()
        {
            return View();
        }
        public static bool UserIsInstructor(string id)
        {
            var db = new ChearnContext();
            return db.Instructors.Where(s
                => s.CUserID == db.CUsers.Where(c
                    => c.AspID == id).FirstOrDefault().ID).FirstOrDefault() != null;
        }

        public static Instructor GetCurrentInstructor(string id)
        {
            var db = new ChearnContext();
            return db.Instructors.Where(s
                => s.CUserID == db.CUsers.Where(c
                    => c.AspID == id).FirstOrDefault().ID).FirstOrDefault();
        }
    }
}