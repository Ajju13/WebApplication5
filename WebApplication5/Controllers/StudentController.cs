using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication5.App_Start;
using WebApplication5.Models;
using System.Globalization;


namespace WebApplication5.Controllers
{
    [CustomAuthorize("S")]
    public class StudentController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Student

        public ActionResult StudentMain(int id)
        {
            // Retrieve the student record using the provided ID
            var student = db.Students.FirstOrDefault(s => s.StudentId == id);

            // Get the student ID from the cookie
            long studentId = GetStudentIdFromCookie(); // Implement GetStudentIdFromCookie method

            // Retrieve marks for the student
            var marks = db.ClassStdResults
                .Where(csr => csr.StudentId == studentId)
                .Select(csr => new GradeHistoryViewModel
                {
                    ClassName = csr.Class.Subject.CourseName,
                    Marks = csr.Marks
                })
                .ToList();

            // Calculate grades and CGPA
            var gradeHistory = CalculateGradesAndCGPA(marks);

            // Calculate total CGPA
            double totalCGPA = CalculateTotalCGPA(gradeHistory);

            // Find the test with the closest test date
            var closestTest = FindClosestTest(studentId);

            // Pass the total CGPA and remaining time of the closest test to the view
            ViewBag.TotalCGPA = totalCGPA;
            ViewBag.NextTestRemainingTime = closestTest != null ? closestTest.RemainingTime : "No Upcoming Test";

            if (student != null)
            {
                // Pass the student model to the view
                return View(student);
            }
            else
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Student record not found.";
                return RedirectToAction("Index");
            }
        }

        // Method to find the closest upcoming test for a student
        private TestViewModel FindClosestTest(long studentId)
        {
            // Retrieve tests for the student
            var tests = db.ClassTests
                .Where(ct => ct.Class.ClassEnrolments.Any(ce => ce.StudentId == studentId))
                .Select(ct => new TestViewModel
                {
                    CourseName = ct.Class.Subject.CourseName,
                    TestType = ct.TestType.TestTypeName,
                    TestDate = ct.TestDate, // Keep test date as string for now
                    MaxMarks = (int)ct.MaxMarks
                })
                .ToList();

            // Calculate remaining time for each test
            foreach (var test in tests)
            {
                test.RemainingTime = CalculateRemainingTime(test.TestDate);
            }

            // Find the test with the closest test date
            return tests.OrderBy(t => t.TestDate).FirstOrDefault(t => t.RemainingTime != "Test Over");
        }

        public ActionResult GetEnrolledCourses()
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
                    case "S":
                        var student = db.Students.FirstOrDefault(s => s.StudentEmail == ticket.Name);
                        if (student != null)
                        {
                            userId = student.StudentId;
                        }
                        break;
                        // Add cases for other user types if needed

                }

                // If user type is student, retrieve enrolled courses for the student
                if (userType == "S")
                {
                    // Retrieve the enrolled courses for the current student
                    var enrolledCourses = db.ClassEnrolments
                        .Where(e => e.StudentId == userId)
                        .Select(e => e.Class.Subject)
                        .ToList();

                    return View(enrolledCourses);
                }
            }

            // If authentication cookie is not found or user type is not student, redirect to unauthorized page or handle accordingly
            return RedirectToAction("Unauthorized", "Home");
        }

        public ActionResult StudentSchedule()
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
                    case "S":
                        var student = db.Students.FirstOrDefault(s => s.StudentEmail == ticket.Name);
                        if (student != null)
                        {
                            userId = student.StudentId;
                        }
                        break;
                        // Add cases for other user types if needed

                }

                // If user type is student, retrieve enrolled courses for the student
                if (userType == "S")
                {
                    // Retrieve the enrolled courses for the current student
                    var enrolledCourses = db.ClassEnrolments
                        .Where(e => e.StudentId == userId)
                        .Select(e => e.Class.Subject)
                        .ToList();

                    return View(enrolledCourses);
                }
            }

            // If authentication cookie is not found or user type is not student, redirect to unauthorized page or handle accordingly
            return RedirectToAction("Unauthorized", "Home");
        }

        public ActionResult RegisterClasses()
        {
            long studentId = GetStudentIdFromCookie(); // Get student ID from cookie
            var enrolledClassIds = db.ClassEnrolments
                                     .Where(e => e.StudentId == studentId)
                                     .Select(e => e.ClassId)
                                     .ToList();

            var availableClasses = db.Classes
                                     .Where(c => !enrolledClassIds.Contains(c.ClassId))
                                     .Select(c => new AvailableClassViewModel
                                     {
                                         ClassId = (int)c.ClassId,
                                         CourseName = c.Subject.CourseName,
                                         ClassDay = c.ClassDay,
                                         ClassSchedule = c.ClassSession.ToString()
                                     })
                                     .ToList();

            var model = new RegisterClassesViewModel
            {
                AvailableClasses = availableClasses
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult RegisterClasses(RegisterClassesViewModel model)
        {
            if (ModelState.IsValid)
            {
                long studentId = GetStudentIdFromCookie(); // Get student ID from cookie

                // Find the maximum existing enrollment ID and increment it by 1
                long maxEnrollmentId = db.ClassEnrolments.Any() ? db.ClassEnrolments.Max(e => e.ClassEnrolmentId) : 0;
                long newEnrollmentId = maxEnrollmentId + 1;

                // Process the selected classes and update the database accordingly
                foreach (var availableClass in model.AvailableClasses)
                {
                    if (availableClass.Selected)
                    {
                        var enrolment = new ClassEnrolment
                        {
                            ClassEnrolmentId = newEnrollmentId, // Use the new enrollment ID
                            StudentId = studentId,
                            ClassId = availableClass.ClassId
                        };
                        db.ClassEnrolments.Add(enrolment);

                        newEnrollmentId++; // Increment the enrollment ID for the next class
                    }
                }

                db.SaveChanges();

                return RedirectToAction("StudentSchedule"); // Redirect to appropriate action after registration
            }

            // If model state is not valid, re-display the view with errors
            return View(model);
        }

        public ActionResult ViewTests()
        {
            // Get the student ID from the cookie
            long studentId = GetStudentIdFromCookie(); // Implement GetStudentIdFromCookie method

            // Retrieve tests for the student
            var tests = db.ClassTests
                .Where(ct => ct.Class.ClassEnrolments.Any(ce => ce.StudentId == studentId))
                .Select(ct => new TestViewModel
                {
                    CourseName = ct.Class.Subject.CourseName,
                    TestType = ct.TestType.TestTypeName,
                    TestDate = ct.TestDate, // Keep test date as string for now
                    MaxMarks = (int)ct.MaxMarks
                })
                .ToList();

            // Calculate remaining time for each test
            foreach (var test in tests)
            {
                test.RemainingTime = CalculateRemainingTime(test.TestDate);
            }

            // Pass the tests to the view
            return View(tests);
        }

        public ActionResult ViewMarks()
        {
            // Get the student ID from the cookie
            long studentId = GetStudentIdFromCookie(); // Implement GetStudentIdFromCookie method

            // Retrieve marks for the current student
            var marks = db.ClassStdResults
                .Where(csr => csr.StudentId == studentId)
                .Select(csr => new MarkViewModel
                {
                    ClassName = csr.Class.Subject.CourseName,
                    Marks = csr.Marks
                })
                .ToList();

            // Pass the marks to the view
            return View(marks);
        }


        private string CalculateRemainingTime(string testDateString)
        {
            // Parse the test date string into a DateTime object
            DateTime testDate;
            if (!DateTime.TryParse(testDateString, out testDate))
            {
                return "Invalid Date Format";
            }

            // Calculate the time difference
            TimeSpan timeDifference = testDate - DateTime.Now;

            // Check if the test date is in the past
            if (timeDifference.TotalSeconds <= 0)
            {
                return "Test Over";
            }

            // Format the remaining time as a string
            string remainingTime = $"{(int)timeDifference.TotalHours} hours {(int)timeDifference.Minutes} minutes";

            return remainingTime;
        }


        private long GetStudentIdFromCookie()
        {
            var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string userType = ticket.UserData;

                if (userType == "S") // Assuming "S" represents a student user
                {
                    var student = db.Students.FirstOrDefault(s => s.StudentEmail == ticket.Name);
                    if (student != null)
                    {
                        return student.StudentId;
                    }
                }
            }

            // Return a default value or handle the case where student is not found
            return -1; // Or throw an exception, redirect to an error page, etc.
        }

        public ActionResult GradeHistory()
        {
            // Get the student ID from the cookie
            long studentId = GetStudentIdFromCookie(); // Implement GetStudentIdFromCookie method

            // Retrieve marks for the student
            var marks = db.ClassStdResults
                .Where(csr => csr.StudentId == studentId)
                .Select(csr => new GradeHistoryViewModel
                {
                    ClassName = csr.Class.Subject.CourseName,
                    Marks = csr.Marks
                })
                .ToList();
            // Calculate grades and CGPA
            var gradeHistory = CalculateGradesAndCGPA(marks);

            double totalCGPA = CalculateTotalCGPA(gradeHistory);

            ViewBag.TotalCGPA = totalCGPA;

            return View(gradeHistory);
        }

        private double CalculateTotalCGPA(List<GradeHistoryViewModel> gradeHistory)
        {
            // Calculate total CGPA by summing up individual CGPAs and dividing by the number of courses
            double totalCGPA = gradeHistory.Sum(g => g.CGPA) / gradeHistory.Count;
            return totalCGPA;
        }
        public List<GradeHistoryViewModel> CalculateGradesAndCGPA(List<GradeHistoryViewModel> marks)

        {
            List<GradeHistoryViewModel> gradeHistory = new List<GradeHistoryViewModel>();


            foreach (var mark in marks)
            {
                string grade;
                double cgpa;

                // Calculate grade based on marks
                if (mark.Marks >= 80)
                {
                    grade = "A";
                }
                else if (mark.Marks >= 70)
                {
                    grade = "B";
                }
                else if (mark.Marks >= 60)
                {
                    grade = "C";
                }
                else if (mark.Marks >= 50)
                {
                    grade = "D";
                }
                else
                {
                    grade = "F";
                }

                // Calculate CGPA assuming all courses are 3 credit hours
                cgpa = mark.Marks * 4 / 100.0;

                // Create GradeViewModel and add to grade history
                var gradeViewModel = new GradeHistoryViewModel
                {
                    ClassName = mark.ClassName,
                    Marks = mark.Marks,
                    Grade = grade,
                    CGPA = cgpa
                };

                gradeHistory.Add(gradeViewModel);
            }

            return gradeHistory;
        }
    }
}