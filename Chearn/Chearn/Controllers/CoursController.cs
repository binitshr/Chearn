using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Chearn.Controllers;
using Chearn.Models;
using Chearn.Models.ViewModels;
using Microsoft.AspNet.Identity;


namespace Chearn.Controllers
{

    public class CoursController : Controller
    {
        private ChearnContext db = new ChearnContext();
        [Authorize(Roles = "Instructor, Admin")]
        // GET: Cours
        public ActionResult Index()
        {
            var aspID = User.Identity.GetUserId();

            var cuser = db.CUsers.Single(i => i.AspID == aspID);

            List<Cours> courses = new List<Cours>();
            var course = db.CourseInstructors.Where(x => x.InstructorID == cuser.ID).ToList();
            foreach(var item in course)
            {

                var temp = db.Courses.Find(item.CourseID);
                courses.Add(temp);
            }
            return View(courses);
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
        public ActionResult Create([Bind(Include = "ID,InstructorID,Name, Tag")] Cours cours)
        {
            if (ModelState.IsValid)
            {
                cours.InstructorID = InstructorController.GetCurrentInstructor(User.Identity.GetUserId()).CUserID;
                HttpPostedFileBase image = Request.Files["ImageData"];
                if(image != null)
                {
                    cours.Picture = ConvertToBytes(image);
                }
                cours.Valid = false;
                cours.Comfirmed = false;
                db.Courses.Add(cours);
                db.SaveChanges();
                db.CourseInstructors.Add(new CourseInstructor { InstructorID = cours.InstructorID, CourseID = cours.ID });
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cours);
        }

        public ActionResult Remove(int id)
        {
            var removed = db.Courses.Find(id);
            removed.Comfirmed = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            System.IO.BinaryReader reader = new System.IO.BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }

        [Authorize(Roles = "Instructor, Admin")]
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
        public ActionResult StudentInfo(int? id) 
        {
            ViewBag.CourseID = id.Value;

            return View();
        }


        [Authorize(Roles = "Instructor, Admin")]
        // POST: Cours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,InstructorID,Name, Valid")] Cours cours)
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

        public JsonResult SetCourseValidationState(int courseID, bool valid)
        {
            var course = db.Courses.Find(courseID);
            course.Valid = valid;
            db.SaveChanges();
            return Json("Validation changed", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> Info(int id) 
        {
            ViewBag.CourseID = id;
            //TODO: implement variable rolling average
            const int RollingAverageDays = 1;
            const int ActiveTimeLimit = 30;
            ActionResult result;
            //Course authenticaion 
            if (!IsAutheticated(id, out result))
                return result;

            var course = await db.Courses.FindAsync(id);
            var model = new AggregateStatsViewModel();
            var courseLessons = db.Lessons.Where(l => l.CourseID == id);
            var courseQuestions = db.Questions.ToList().Where(q => courseLessons.Where(c => c.ID == q.LessonID.Value).Any()).ToList();
            var courseReviews = db.Reviews.ToList().Where(r => courseQuestions.Where(q => q.ID == r.QuestionID).Any()).ToList();

            model.ID = course.ID;
            model.CourseName = course.Name;
            model.ReviewDays = RollingAverageDays;
            var enrollmentList = await db.Enrollments.Where(e => e.CourseID.Value == id).ToListAsync();
            model.TotalStudents = enrollmentList.Select(e => e.StudentID).Count();
            //active student is defined as a student whose last review was less than (ActiveTimeLimit) days 
            var test = (await ReviewController.GetLatestReviews(id, ActiveTimeLimit)).ToList();
            model.ActiveStudents = model.TotalStudents;//(await ReviewController.GetLatestReviews(id, ActiveTimeLimit)).ToList().GroupBy(r => r.UserID).Count();

            model.CorrectRate = courseReviews.Where(r => r.IsCorrect).Count() / (double)courseReviews.Count();
            model.ChallengeQuestion = new List<Question>();
            foreach(var question in courseQuestions)
            {
                var totalCount = courseReviews.Where(r => r.QuestionID == question.ID).Count();
                var correctCount = courseReviews.Where(r => (r.IsCorrect && r.QuestionID == question.ID)).Count();
                if (totalCount > 0 && ( correctCount / (double)totalCount ) < model.CorrectRate)
                {
                    model.ChallengeQuestion.Add(question);
                }
            }
            List<double> dailyAverages = new List<double>(RollingAverageDays);
            for(int i = 0; i < RollingAverageDays; i++)
            {
                double val = courseReviews.Where(r => r.TimeStamp < DateTime.Now.AddDays(-i) && r.IsCorrect).Count() 
                    / (double)courseReviews.Where(r => r.TimeStamp < DateTime.Now.AddDays(-i)).Count();
                dailyAverages.Add(val);
            }
            model.SMAReviewsPerDay = (dailyAverages.Sum()/dailyAverages.Count());
            


            
            model.ReviewDays = 10;
            return View(model);
        }

        [Authorize(Roles = "Instructor, Admin")]
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

        [Authorize(Roles = "Instructor, Admin")]
        // POST: Cours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) { 
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

            var temp = db.CourseInstructors.Where(i => i.CourseID == id.Value && i.InstructorID == instructor.ID).ToList();
            if (temp.Count == 0)
            {
                errorResult = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "This is not your course to edit");
            }
            if (errorResult != null)
                return false;

            return true;
        }

        [Authorize(Roles = "Instructor, Admin")]
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


        [Authorize(Roles = "Student, Admin")]
        public ActionResult StudentCourses()
        {
           
            var studentID = StudentsController.GetCurrentStudent(User.Identity.GetUserId()).ID;

            var courseIDs = db.Enrollments.Where(c => c.StudentID == studentID).Select(c=>c.CourseID);

            var AvailableCourses = db.Courses.Where(c => courseIDs.Contains(c.ID)).ToList();

            return View(AvailableCourses);
        }

        


        [Authorize(Roles = "Student")]
        public ActionResult StudentCourse(int? id)
        {
             
            ViewBag.CourseID = id.Value;
            var studentID = User.Identity.GetUserId();

            var CompletionRate = 40;


            /**
             * var LessonTotal = db.Lessons.Where(x => x.CourseID == id.Value).Count();

            var QuestionIDs = db.Reviews.Where(x => x.UserID == studentID).Select(x =>x.QuestionID).Distinct();

            var lessonIDs = db.Questions.Where(x => QuestionIDs.Contains(x.ID)).Select(x => x.LessonID).Distinct();

            var CompletedLessons = db.Lessons.Where(x => lessonIDs.Contains(x.ID)).Count();

            
            if (CompletedLessons != 0)
            {
                var CompletionRate = LessonTotal / CompletedLessons;

                ViewBag.rate = CompletionRate;

            }
            else {
                ViewBag.rate = 10 ;
            }

             */



            ViewBag.rate = CompletionRate;

            var course = db.Courses.Find(id);
            return View(course);
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

   

        [HttpGet]
        public ActionResult CourseShare(int? id)
        {
            var course = db.Courses.Find(id.Value);
            var Teachers = db.Instructors.Select(x => new CourseSharingModel()
            {
                ID = x.ID,
                CID = id.Value,
                FirstName = x.CUser.FirstName,
                LastName = x.CUser.LastName
            }).Where(x => x.CID == id.Value).ToList();
            return View(Teachers);
       }

        [HttpPost]
        public ActionResult CourseShare(CourseSharingModel person, string add, string remove)
        {
            if (!string.IsNullOrEmpty(add))
            {
                var temp = db.CourseInstructors.Where(x => x.CourseID == person.CID && x.InstructorID == person.ID).ToList();
                if (temp.Count == 0)
                {
                    db.CourseInstructors.Add(new CourseInstructor { InstructorID = person.ID, CourseID = person.CID });
                    db.SaveChanges();
                }
            }else if (!string.IsNullOrEmpty(remove))
            {
                var temp = db.CourseInstructors.Where(x => x.CourseID == person.CID && x.InstructorID == person.ID).ToList();
                if(temp.Count != 0)
                {
                    foreach (var item in temp)
                    {
                        db.CourseInstructors.Remove(item);
                    }
                    db.SaveChanges();
                }
            }
            var course = db.Courses.Find(person.CID);
            var Teachers = db.Instructors.Select(x => new CourseSharingModel()
            {
                ID = x.ID,
                CID = person.CID,
                //FirstName = course.Instructor.CUser.FirstName,
                FirstName = x.CUser.FirstName,
                //LastName = course.Instructor.CUser.LastName
                LastName = x.CUser.LastName
            }).Where(x => x.CID == person.CID).ToList();
            return View(Teachers);
        }

        public JsonResult GetEnrolledStudents(int? id)
        {
            var studentIDs = db.Enrollments.Where(r => r.CourseID == id.Value).Select(c => c.StudentID).ToList();
            var students = db.Students.Where(s => studentIDs.Contains(s.ID)).Select(s => new { s.ID, s.CUser.FirstName, s.CUser.LastName}).ToList();
            return Json(students, JsonRequestBehavior.AllowGet);
        }
    } 
}

