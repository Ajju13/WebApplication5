using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.App_Start;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class CourseController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult AddCourse()
        {
            var newCourse = new Subject();

            // Get the maximum Course_ID from the database
            var maxCourseId = db.Courses.Max(c => (int?)c.CourseId) ?? 0;

            // Set the Course_ID for the new course as the maximum ID + 1
            newCourse.CourseId = maxCourseId + 1;

            return View(newCourse);
        }
        [HttpPost]
        public ActionResult AddCourse(Subject c)
        {
            if (ModelState.IsValid)
            {
                Subject c1 = new Subject();

                c1.CourseId = c.CourseId;
                c1.CourseName = c.CourseName;
                c1.PreRequisiteCourseId = null;
                c1.CreditHours = c.CreditHours;

                db.Courses.Add(c1);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Course Added Successfully";
                return View(c1);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ViewCourse()
        {
            var courses = db.Courses.ToList(); // Retrieve all students from the database
            return View(courses);
        }
    }
}