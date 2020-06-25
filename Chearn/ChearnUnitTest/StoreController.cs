using Chearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chearn.Controllers
{
    public class StoreController : Controller
    {
        private ChearnContext db = new ChearnContext();

        private bool FindCustomView(string name)
        {
            return ViewEngines.Engines.FindView(ControllerContext, name, null).View != null;
        }

        // GET: Store
        public ActionResult Index()
        {
            ViewBag.categories = db.Categories.ToList();

            return View(db.ShopItems.ToList());
        }

        public ActionResult Item(int v)
        {
            var itemCourses = db.ShopItemCourses.Where(itemCourse => itemCourse.ItemID == v).Select(itemCourse => itemCourse.CourseID).ToList();
            ViewBag.includedCourses = db.Courses.Select(course => course).Where(course => itemCourses.Contains(course.ID)).ToArray();
            return View(db.ShopItems.Find(v));
        }

        public ActionResult Category(int c)
        {
            ViewBag.categories = db.Categories.ToList();

            var catName = db.Categories.Where(cat => cat.ID == c).Select(cat => cat.Name).FirstOrDefault();
            var customView = FindCustomView(catName);
            var shopItems = db.ShopItems.Where(item => item.Category == c).Select(item => item).ToList();

            if (customView)
            {
                return View(catName, shopItems);
            }

            return View("Index", shopItems);
        }

    }
}