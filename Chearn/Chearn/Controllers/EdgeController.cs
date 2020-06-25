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
using Newtonsoft.Json;

namespace Chearn.Controllers
{
    public class EdgeController : Controller
    {
        private ChearnContext db = new ChearnContext();
        private bool HasCycle(ICollection<Edge> edges)
        {
            if (edges.Count == 0)
                return false;
            var lessons = edges.Select(e => e.ChildID).Distinct().ToList();
            lessons.AddRange(edges.Where(e => !lessons.Contains(e.ParentID)).Select(e => e.ParentID).ToList());
            var childNodes = new List<int?>();
            var childlessNodes = new List<int?>();
            foreach(var lesson in lessons)
            {
                foreach(var edge in edges)
                {
                    if (edge.ChildID == lesson && !childNodes.Contains(edge.ChildID))
                        childNodes.Add(edge.ChildID);
                }
            }

            foreach(var lesson in lessons)
            {
                if (!childNodes.Contains(lesson))
                    childlessNodes.Add(lesson);
            }
            return !childlessNodes.Any();
        }
        [HttpPost]
        public JsonResult HandleEdgeSave(int courseID, string edgesToAddJson, string edgesToRemoveJson)
        {
            EdgeDTO[] edgesToAdd = { };
            EdgeDTO[] edgesToRemove = { };
            if (edgesToAddJson != null)
                edgesToAdd = JsonConvert.DeserializeObject<EdgeDTO[]>(edgesToAddJson);
            if (edgesToRemoveJson != null)
                edgesToRemove = JsonConvert.DeserializeObject<EdgeDTO[]>(edgesToRemoveJson);
            
            var course = db.Courses.Find(courseID);

            //TODO: maybe write code ensuring edges don't already exist if not done clientside
            if (course != null && course.Instructor.CUser.AspID == User.Identity.GetUserId())
            {
                foreach (var edgeDTO in edgesToAdd)
                {
                        db.Edges.Add(edgeDTO.ToEdge());
                }
                foreach (var edgeDTO in edgesToRemove)
                {
                    var removeEdges = db.Edges.ToArray();
                    
                    removeEdges = removeEdges.Where(e => e.ParentID == edgeDTO.ToEdge().ChildID && e.ChildID == edgeDTO.ToEdge().ParentID).ToArray();
                    db.Edges.RemoveRange(removeEdges);
                }
                db.SaveChanges();
                var lessons = db.Lessons.Where(l => l.CourseID == courseID).Select(l => l.ID).ToList();
                var allEdges = db.Edges.Where(e => lessons.Contains(e.ChildID.Value) || lessons.Contains(e.ParentID.Value)).ToList();

                if (!HasCycle(allEdges))
                {
                    return Json(new { message = "" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { message = "Your lesson plan may contain cycles. Consider revising the curriculum." }, JsonRequestBehavior.AllowGet);
                }
                
            }
            return Json(":(", JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddEdge(int courseID, int parentID, int childID)
        {
            var course = db.Courses.Find(courseID);
            if (course != null && course.Instructor.CUser.AspID == User.Identity.GetUserId())
            {
                //ensures the lessons belong to the right course
                if (course.Lessons.Where(l => l.ID == parentID).Count() < 1 && course.Lessons.Where(l => l.ID == childID).Count() < 1)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                if (db.Edges.Where(e => e.ParentID == parentID && e.ChildID == childID).Count() > 0)
                {
                    return Json(new { status = "edge already exists!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Edges.Add(new Edge() { ChildID = childID, ParentID = parentID });
                    return Json(new { status = "added succesfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveAllEdges(int courseID)
        {
            var course = db.Courses.Find(courseID);
            if (course != null && course.Instructor.CUser.AspID == User.Identity.GetUserId())
            {
                var lessons = db.Lessons.Where(l => l.CourseID == courseID);
                var edgeList = new List<Edge>();
                foreach (var lesson in lessons)
                {
                    edgeList.AddRange(db.Edges.Where(e => e.ChildID == lesson.ID || e.ParentID == lesson.ID));
                }
                db.Edges.RemoveRange(edgeList);
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllEdges(int courseID)
        {
            var course = db.Courses.Find(courseID);
            var preLessonsArr = course.Lessons.ToArray();
            var lessonsArr = new int[preLessonsArr.Length];
            for(int i = 0; i < lessonsArr.Length; i++)
            {
                lessonsArr[i] = preLessonsArr[i].ID;
            }

            if (course != null)
            {
                //TODO: optimize someday to avoid N^2 lookups into lessons
                return Json(db.Edges.Where(e =>lessonsArr.Where(l => l == e.ParentID || l == e.ChildID).Count() > 0).Select(edge => new { edge.ID, edge.ParentID, edge.ChildID}).ToArray(), JsonRequestBehavior.AllowGet);
                
            }
            return Json(new List<Edge>(), JsonRequestBehavior.AllowGet);
        }

       

    }
}