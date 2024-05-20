using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication5.App_Start;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    [CustomAuthorize("T")]
    public class FacultyController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Faculty
        public ActionResult FacultyMain(int id)
        {
            // Retrieve the student record using the provided ID
            var Faculty = db.Faculties.FirstOrDefault(s => s.FacultyId == id);

            if (Faculty != null)
            {
                int totalClasses = db.Classes.Count(c => c.FacultyId == Faculty.FacultyId);

                // Pass the faculty model and total number of classes to the view
                ViewBag.TotalClasses = totalClasses;
                return View(Faculty);
            }
            else
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Faculty record not found.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult FacultyClasses()
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;
                long userId = -1;

                if (userType == "T")
                {
                    var faculty = db.Faculties.FirstOrDefault(f => f.FacultyEmail == ticket.Name);
                    if (faculty != null)
                    {
                        userId = faculty.FacultyId;
                    }

                    if (userId != -1)
                    {
                        var facultyClasses = db.Classes
                            .Where(c => c.FacultyId == userId)
                            .Select(c => new FacultyClassViewModel
                            {
                                Class = c,
                                NumberOfUsersEnrolled = db.ClassEnrolments.Count(e => e.ClassId == c.ClassId)
                            })
                            .ToList();

                        return View(facultyClasses);
                    }
                }
            }

            return RedirectToAction("Unauthorized", "Home");
        }


        public ActionResult FacultySchedule()
        {
            // Get the user type and ID from Forms Authentication
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;
                long userId = -1; // Initialize userId to a default value

                // Query the database to get the corresponding ID based on user type
                switch (userType)
                {
                    case "T":
                        var faculty = db.Faculties.FirstOrDefault(s => s.FacultyEmail == ticket.Name);
                        if (faculty != null)
                        {
                            userId = faculty.FacultyId;
                        }
                        break;
                        // Add cases for other user types if needed
                }

                // If user type is faculty, retrieve classes taught by the faculty
                if (userType == "T")
                {
                    // Retrieve the classes taught by the current faculty
                    var facultyClasses = db.Classes
                        .Where(c => c.FacultyId == userId)
                        .Include(c => c.Subject) // Include Subject navigation property to avoid lazy loading
                        .ToList();

                    return View(facultyClasses);
                }
            }

            // If authentication cookie is not found or user type is not faculty, redirect to unauthorized page or handle accordingly
            return RedirectToAction("Unauthorized", "Home");
        }


        [HttpPost]
        public ActionResult AddTest(Class_Test model)
        {
            if (ModelState.IsValid)
            {
                var maxTestId = db.ClassTests.Any() ? db.ClassTests.Max(t => t.CTId) : 0;
                model.CTId = maxTestId + 1;

                db.ClassTests.Add(model);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Test added successfully.";
                return RedirectToAction("AddTest");
            }

            PopulateViewBags();

            return View(model);
        }

        private void PopulateViewBags()
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;
                long userId = -1;

                if (userType == "T")
                {
                    var faculty = db.Faculties.FirstOrDefault(f => f.FacultyEmail == ticket.Name);
                    if (faculty != null)
                    {
                        userId = faculty.FacultyId;
                    }

                    var facultyClasses = db.Classes
                        .Where(c => c.FacultyId == userId)
                        .Select(c => new
                        {
                            ClassId = c.ClassId,
                            CourseName = c.Subject.CourseName
                        })
                        .ToList()
                        .Select(c => new
                        {
                            c.ClassId,
                            DisplayText = $"{c.CourseName} ({c.ClassId})"
                        })
                        .ToList();

                    ViewBag.Classes = new SelectList(facultyClasses, "ClassId", "DisplayText");
                }
            }

            var testTypes = db.TestTypes.ToList();
            ViewBag.TestTypes = new SelectList(testTypes, "TestTypeId", "TestTypeName");
        }

        public ActionResult AddTest()
        {
            PopulateViewBags();
            return View(new Class_Test());
        }
        public ActionResult ViewAllTest()
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;
                long userId = -1;

                // Query the database to get the corresponding ID based on user type
                if (userType == "T")
                {
                    var faculty = db.Faculties.FirstOrDefault(f => f.FacultyEmail == ticket.Name);
                    if (faculty != null)
                    {
                        userId = faculty.FacultyId;

                        // Retrieve tests for the classes taught by the faculty
                        var tests = db.ClassTests
                            .Where(t => t.Class.FacultyId == userId)
                            .Select(t => new ClassTestViewModel
                            {
                                CT_id = t.CTId,
                                CourseName = t.Class.Subject.CourseName,
                                ClassSession = t.Class.ClassSession.ToString(),
                                TestTypeName = t.TestType.TestTypeName,
                                TestDate = t.TestDate,
                                MaxMarks = t.MaxMarks
                            })
                            .ToList();

                        return View(tests);
                    }
                }
            }

            // If authentication cookie is not found or user type is not faculty, redirect to unauthorized page or handle accordingly
            return RedirectToAction("Unauthorized", "Home");

        }

        public ActionResult ViewAllTestTypes()
        {
            var a = db.TestTypes.ToList();
            return View(a);
        }

        public ActionResult GradeTest()
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;

                if (userType == "T")
                {
                    var faculty = db.Faculties.FirstOrDefault(f => f.FacultyEmail == ticket.Name);
                    if (faculty != null)
                    {
                        var classes = db.Classes
                            .Where(c => c.FacultyId == faculty.FacultyId)
                            .Select(c => new
                            {
                                ClassId = c.ClassId,
                                CourseName = c.Subject.CourseName
                            })
                            .ToList();

                        ViewBag.Classes = new SelectList(classes, "ClassId", "CourseName"); // Change "Class_ID" to "ClassId"
                        return View();
                    }
                }
            }

            return RedirectToAction("Unauthorized", "Home");
        }

        [HttpPost]
        public ActionResult GradeTest(long selectedClassId)
        {
            var students = db.ClassEnrolments
                .Where(e => e.ClassId == selectedClassId)
                .Select(e => new StudentGradeViewModel
                {
                    Student_ID = e.Student.StudentId,
                    StudentName = e.Student.StudentName,
                    Marks = 0 // Default value
        })
                .ToList();

            var model = new GradeTestViewModel
            {
                Class_ID = selectedClassId,
                CourseName = db.Classes.FirstOrDefault(c => c.ClassId == selectedClassId)?.Subject.CourseName,
                Students = students
            };

            return View("GradeTestForm", model);
        }

        [HttpPost]
        public ActionResult SaveGrades(GradeTestViewModel model)
        {
            var maxFacultyId = db.ClassStdResults.Max(u => (int?)u.CSRId) ?? 0;
            long csrnewid = maxFacultyId + 1;

            foreach (var student in model.Students)
            {
                var classStdResult = new Class_StdResult
                {
                    CSRId = csrnewid,
                    ClassId = model.Class_ID,
                    StudentId = student.Student_ID,
                    Marks = student.Marks
                };
                db.ClassStdResults.Add(classStdResult);
            }



            db.SaveChanges();

            return RedirectToAction("ViewAllTest");
        }

        public ActionResult ViewTestResult()
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;

                if (userType == "T")
                {
                    var faculty = db.Faculties.FirstOrDefault(f => f.FacultyEmail == ticket.Name);
                    if (faculty != null)
                    {
                        var classes = db.Classes
                            .Where(c => c.FacultyId == faculty.FacultyId)
                            .Select(c => new
                            {
                                ClassId = c.ClassId,
                                CourseName = c.Subject.CourseName
                            })
                            .ToList();

                        ViewBag.Classes = new SelectList(classes, "ClassId", "CourseName"); // Change "Class_ID" to "ClassId"
                        return View();
                    }
                }
            }

            return RedirectToAction("Unauthorized", "Home");
        }

        [HttpPost]
        public ActionResult ViewTestResult(long selectedClassId)
        {
            var students = db.ClassStdResults
                .Where(r => r.ClassId == selectedClassId)
                .Select(r => new StudentGradeViewModelForFaculty
                {
                    Student_ID = r.Student.StudentId,
                    StudentName = r.Student.StudentName,
                    Marks = r.Marks
                })
                .ToList();

            var model = new ViewTestResultViewModel
            {
                Class_ID = selectedClassId,
                CourseName = db.Classes.FirstOrDefault(c => c.ClassId == selectedClassId)?.Subject.CourseName,
                Students = students
            };

            ViewBag.Classes = new SelectList(db.Classes.Where(c => c.FacultyId == db.Faculties.FirstOrDefault(f => f.FacultyEmail == User.Identity.Name).FacultyId)
                .Select(c => new { c.ClassId, CourseName = c.Subject.CourseName }).ToList(), "ClassId", "CourseName", selectedClassId); // Change "Class_ID" to "ClassId"

            return View("TestResultList", model);

        }


        [HttpPost]
        public ActionResult SaveEditedGrades(ViewTestResultViewModel model)
        {
            foreach (var student in model.Students)
            {
                var result = db.ClassStdResults.FirstOrDefault(r => r.ClassId == model.Class_ID && r.StudentId == student.Student_ID);
                if (result != null)
                {
                    result.Marks = student.Marks;
                    db.Entry(result).State = System.Data.Entity.EntityState.Modified;
                }
            }

            db.SaveChanges();

            return RedirectToAction("ViewTestResult");
        }
    }
}