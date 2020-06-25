using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Chearn.Models;
using Chearn.Models.ViewModels;
using Chearn.Models.dtos;
using Microsoft.AspNet.Identity;
namespace Chearn.Controllers
{
    public class ReviewController : Controller
    {
        private ChearnContext db = new ChearnContext();
        

        //TODO: implement review feature
        public JsonResult GetReviews()
        {
            return null;
        }

        public async Task<ActionResult> LessonPage(int? lessonID)
        {
            var lesson = await db.Lessons.Where(l => l.ID == lessonID).FirstOrDefaultAsync();
            return View(lesson);
        }
        [HttpGet]
        public async Task<ActionResult> QuizPage(int? lessonID)
        {
            var question = await db.Questions.Where(q => q.LessonID.Value == lessonID).FirstOrDefaultAsync();
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuizPage(int? lessonID, string answer)
        {
            var question = await db.Questions.Where(q => q.ID == lessonID.Value).FirstOrDefaultAsync();
            var realAnswer = question.Answer.ToLower().Trim();
            //if the two answers are the same minus white space and caps
            if (CorrectAnswer(answer, realAnswer))
            {
                var userID = User.Identity.GetUserId();
                var review = await db.Reviews.Where(r => r.QuestionID == question.ID && r.UserID == userID).FirstOrDefaultAsync();
                if (review == null)
                {
                    db.Reviews.Add(new Review() { IsCorrect = true, Level = 1, QuestionID = question.ID, UserID = userID, TimeStamp = DateTime.Now });
                    await db.SaveChangesAsync();
                }
                return Redirect("/Home/Index");
            }
            else
            {
                ViewBag.Message = "Sorry, that's not quite right! Keep trying until you get it right!";
                return View(question);
            }
        }

        private static bool CorrectAnswer(string userAnswer, string realAnswer)
        {
            return userAnswer.ToLower().Trim().Equals(realAnswer.ToLower().Trim());
        }

        public ActionResult ReviewCenter()
        {
            return View();
        }
        public static async Task<List<Review>> GetLatestReviews(int courseID, int daysBack)
        {
            var db = new ChearnContext();
            var currentDate = DateTime.Now;
            var lessons = await db.Lessons.Where(lesson => lesson.CourseID == courseID).ToListAsync();
            var lessonQuestions = db.Questions.ToList().Where(q => lessons.Where(l => l.ID == q.LessonID).Any()).Select(q => q.ID).ToList();

            return db.Reviews.Where(r => lessonQuestions.Contains(r.QuestionID)).OrderBy(r
               => r.QuestionID).ThenBy(r => r.TimeStamp).GroupBy(r => r.QuestionID).ToList().Select(g =>
                   g.LastOrDefault()).ToList().Where(review => (currentDate - review.TimeStamp).TotalDays < daysBack).ToList();
        }
        
        public JsonResult GetAllReviews()
        {
            var userID = User.Identity.GetUserId();
            var currentDate = DateTime.Now;
            var reviews = db.Reviews.Where(r => r.UserID == userID).OrderBy(r
                => r.QuestionID).ThenBy(r => r.TimeStamp).GroupBy(r => r.QuestionID).ToList().Select(g =>
                    g.LastOrDefault()).ToList().Where(review => (currentDate - review.TimeStamp).TotalDays > ComputeNextReviewTime(review)).ToList();

            return Json(reviews, JsonRequestBehavior.AllowGet);
        }

        public static async Task<List<Review>> GetStudentReviews(int studentID, int courseID)
        {
            var db = new ChearnContext();
            var student = await db.Students.FindAsync(studentID);
            var cUser = await db.CUsers.FindAsync(student.CUserID);
            var course = await db.Courses.FindAsync(courseID);
            var lessons = await db.Lessons.Where(l => l.CourseID == courseID).ToListAsync();
            var questions = (await db.Questions.ToListAsync()).Where(q => lessons.Select(l => l.ID).ToList().Contains(q.LessonID.Value));
            var reviews = (await db.Reviews.ToListAsync())
                .Where(r => questions.Select(q => q.ID).ToList().Contains(r.QuestionID) && r.UserID == cUser.AspID).ToList();
            return reviews;
        }

        public static double ComputeNextReviewTime(Review review)
        {
            const int reviewTimeInterval = 0;
            return Math.Pow(reviewTimeInterval, review.Level);
        }

        public JsonResult GetQuestionForReview(int questionID)
        {
            var question = db.Questions.Find(questionID);
            var rQuestion = new { question.ID, question.Text, question.Answer };
            return Json(rQuestion, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckQuestion(int? questionID, string answer)
        {
            var userID = User.Identity.GetUserId();
            var question = db.Questions.Find(questionID.Value);
            var levelTask = db.Reviews.Where(r => r.QuestionID == questionID.Value && r.UserID == userID).FirstOrDefaultAsync();
            //await Task.WhenAll(questionTask, levelTask);
            //await questionTask;
            await levelTask;
            var level = levelTask.Result.Level;
            var result = new ReviewDTO() { QuestionID = questionID.Value, LessonID = question.LessonID.Value, IsCorrect = CorrectAnswer(answer, question.Answer) };
                db.Reviews.Add(new Review() { IsCorrect = result.IsCorrect, QuestionID = questionID.Value, 
                    Level = result.IsCorrect ? ++level : level > 1 ? --level : level, TimeStamp = DateTime.Now, UserID = User.Identity.GetUserId() });
                await db.SaveChangesAsync();
            return Json(result, JsonRequestBehavior.AllowGet);
            
        }
    }
}