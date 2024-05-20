using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.App_Start;
using WebApplication5.Models;
using System.Data.Entity;

namespace WebApplication5.Controllers
{

    public class ClassController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Class
        public ActionResult AddClass()
        {
            var newClass = new Class();

            // Get the maximum Class_ID from the database
            var maxClassId = db.Classes.Max(u => (int?)u.ClassId) ?? 0;

            // Set the Class_ID for the new class as the maximum ID + 1
            newClass.ClassId = maxClassId + 1;

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            ViewBag.FacultyId = new SelectList(db.Faculties, "FacultyId", "FacultyName");

            return View(newClass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClass(Class cls)
        {
            // Check if the provided Class ID already exists
            var existingClass = db.Classes.Find(cls.ClassId);
            if (existingClass != null)
            {
                ModelState.AddModelError("Class_ID", "Class ID already exists.");
            }

            if (ModelState.IsValid)
            {
                db.Classes.Add(cls);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Class added successfully.";
                return RedirectToAction("ViewClass", "Class");
            }

            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name", cls.CourseId);
            // Other ViewBag assignments for dropdown lists

            return View(cls);
        }

        public ActionResult ViewClass()
        {
            // Include the related Course and Faculty entities
            var classes = db.Classes.Include("Subject").Include("Faculty").ToList();
            return View(classes);
        }

        public ActionResult CourseSelection()
        {
            var classes = db.Classes.Include(c => c.Subject).ToList();
            var studentList = db.Students.ToList();

            // Convert the list of classes to a list of SelectListItem
            var classListItems = classes.Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = c.Subject != null ? $"{c.Subject.CourseName} ({c.ClassId})" : $"No Course ({c.ClassId})"
            }).ToList();

            var studentSelectList = studentList.Select(s => new SelectListItem
            {
                Value = s.StudentId.ToString(),
                Text = $"{s.StudentName} ({s.StudentId})"
            }).ToList();

            // Add a default option if needed
            classListItems.Insert(0, new SelectListItem { Value = "", Text = "Select a class" });

            // Add the list of SelectListItem to ViewData with the key 'ClassId'
            ViewData["ClassId"] = classListItems;

            // Add the SelectListItem collection to ViewData with the appropriate key
            ViewData["StudentList"] = studentSelectList;

            // Find the highest ClassEnrolmentId and increment by 1
            var highestEnrolmentId = db.ClassEnrolments.Max(c => (int?)c.ClassEnrolmentId) ?? 0;
            var nextEnrolmentId = highestEnrolmentId + 1;

            // Create a new instance of ClassEnrolment model to pass to the view
            var model = new ClassEnrolment
            {
                ClassEnrolmentId = nextEnrolmentId
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult CourseSelection(ClassEnrolment c)
        {
            if (ModelState.IsValid)
            {
                // Add the provided Class_Enrolment object 'c' to the database context
                db.ClassEnrolments.Add(c);
                db.SaveChanges();

                // Redirect to a different action or view
                return RedirectToAction("ViewClass", "Class"); // Example: Redirect to the home page
            }
            // If the model state is not valid, return the same view with validation errors
            return View(c);
        }
    }
}