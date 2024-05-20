using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;
using WebApplication5.App_Start;
using System.IO;
using System.Data.Entity.Validation;


namespace WebApplication5.Controllers
{
    [CustomAuthorize("A")]
    public class AdminController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the user already exists
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "User with this email already exists.";
                    return View(user);
                }

                // Find the highest existing ID in the Authorization table
                long highestId = db.Users.Max(u => (long?)u.UserId) ?? 0;

                // Increment the ID for the new user
                user.UserId = highestId + 1;

                db.Users.Add(user);
                db.SaveChanges();

                // Display success message
                TempData["SuccessMessage"] = "User added successfully.";
                return View(user);
            }

            return View(user);
        }

        public ActionResult ViewUser()
        {
            var User = db.Users.ToList(); // Retrieve all students from the database
            return View(User);
        }

        public ActionResult EditUser(int id)
        {
            var User = db.Users.FirstOrDefault(s => s.UserId == id);

            if (User == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "User record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(User);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User editedUser)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingUser = db.Users.FirstOrDefault(s => s.UserId == editedUser.UserId);

                if (existingUser != null)
                {
                    // Update student information with edited values
                    existingUser.Email = editedUser.Email;
                    existingUser.Password = editedUser.Password;
                    existingUser.UserType = editedUser.UserType;


                    // Save changes to the database
                    db.SaveChanges();

                    // Display success message
                    TempData["SuccessMessage"] = "User information updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User record not found.";
                }
            }
            else
            {
                // If ModelState is not valid, return the view with the invalid model
                return View(editedUser);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("ViewUser", "Admin", new { id = editedUser.UserId });
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound(); // or handle appropriately if user is not found
            }

            if (user.UserType == "S")
            {
                // If the user is a student, delete all records associated with the student
                var student = db.Students.FirstOrDefault(s => s.StudentEmail == user.Email);
                if (student != null)
                {
                    // Delete all class enrollments associated with the student
                    var enrollments = db.ClassEnrolments.Where(e => e.StudentId == student.StudentId);
                    db.ClassEnrolments.RemoveRange(enrollments);
                    // Now delete the student record
                    db.Students.Remove(student);
                }
            }
            else if (user.UserType == "T" || user.UserType == "A")
            {
                // If the user is a faculty or admin, delete their record directly
                if (user.UserType == "T")
                {
                    // Delete faculty record
                    var faculty = db.Faculties.FirstOrDefault(f => f.FacultyEmail == user.Email);
                    if (faculty != null)
                    {
                        // Find all classes associated with the faculty
                        var classesToDelete = db.Classes.Where(c => c.FacultyId == faculty.FacultyId);

                        // Delete each class
                        foreach (var cls in classesToDelete)
                        {
                            db.Classes.Remove(cls);
                        }

                        // Delete the faculty record
                        db.Faculties.Remove(faculty);
                    }
                }
                else if (user.UserType == "A")
                {
                    // Delete admin record
                    var admin = db.Staffs.FirstOrDefault(a => a.StaffEmail == user.Email);
                    if (admin != null)
                    {
                        db.Staffs.Remove(admin);
                    }
                }
            }

            // Now delete the user record
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("ViewUser"); // Redirect back to the same page
        }

        public ActionResult AddStudent()
        {
            var newStudent = new Student();

            // Get the maximum Student_ID from the database
            var maxStudentId = db.Students.Max(u => (int?)u.StudentId) ?? 0;

            // Set the Student_ID for the new student as the maximum ID + 1
            newStudent.StudentId = maxStudentId + 1;

            return View(newStudent);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(Student s1, HttpPostedFileBase imgfile)

        {
            if (ModelState.IsValid)
            {

                // Check if the user already exists
                var existingUser = db.Students.FirstOrDefault(u => u.StudentEmail == s1.StudentEmail);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "User with this email already exists.";
                    return View(s1);
                }

                // Get the maximum Student_ID from the database
                var maxStudentId = db.Students.Max(u => (int?)u.StudentId) ?? 0;

                // Increment the maximum Student_ID by 1
                s1.StudentId = maxStudentId + 1;

                Student sl = new Student();
                string path = UploadImage(imgfile);
                if (path.Equals("-1"))
                {

                }
                else
                {
                    try
                    {
                        sl.StudentId = s1.StudentId;
                        sl.StudentName = s1.StudentName;
                        sl.StudentEmail = s1.StudentEmail;
                        sl.StudentAddress = s1.StudentAddress;
                        sl.StudentPhone = s1.StudentPhone;
                        sl.StudentImage = path;

                        db.Students.Add(sl);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Student added successfully.";
                        return RedirectToAction("AddStudent", "Admin"); // Redirect to a different action
                    }
                    catch (DbEntityValidationException ex)
                    {
                        // Handle validation errors
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }


            }

            // If ModelState is not valid, return the view with the invalid model
            return View(s1);
        }

        public ActionResult ViewStudents()
        {
            var students = db.Students.ToList(); // Retrieve all students from the database
            return View(students);
        }

        public ActionResult EditStudent(int id)
        {
            var student = db.Students.FirstOrDefault(s => s.StudentId == id);

            if (student == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Student record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(Student editedStudent)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingStudent = db.Students.FirstOrDefault(s => s.StudentId == editedStudent.StudentId);

                if (existingStudent != null)
                {
                    // Update student information with edited values
                    existingStudent.StudentId = editedStudent.StudentId;
                    existingStudent.StudentName = editedStudent.StudentName;
                    existingStudent.StudentEmail = editedStudent.StudentEmail;
                    existingStudent.StudentAddress = editedStudent.StudentAddress;
                    existingStudent.StudentPhone = editedStudent.StudentPhone;


                    // Save changes to the database
                    db.SaveChanges();

                    // Display success message
                    TempData["SuccessMessage"] = "Student information updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Student record not found.";
                }
            }
            else
            {
                // If ModelState is not valid, return the view with the invalid model
                return View(editedStudent);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("ViewStudents", "Admin", new { id = editedStudent.StudentId });
        }

        [HttpPost]
        public ActionResult DeleteStudent(int id)
        {
            var student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound(); // or handle appropriately if student is not found
            }

            // Delete all class enrollments associated with the student
            var enrollments = db.ClassEnrolments.Where(e => e.StudentId == id);
            db.ClassEnrolments.RemoveRange(enrollments);

            // Now delete the student record
            db.Students.Remove(student);
            db.SaveChanges();

            return RedirectToAction("ViewStudents"); // Redirect back to the same page
        }

        public ActionResult AddAdmin()
        {
            var newAdmin = new Staff();

            // Get the maximum Student_ID from the database
            var maxAdminId = db.Staffs.Max(u => (int?)u.StaffId) ?? 0;

            // Set the Student_ID for the new student as the maximum ID + 1
            newAdmin.StaffId = maxAdminId + 1;

            return View(newAdmin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(Staff s1, HttpPostedFileBase imgfile)

        {
            if (ModelState.IsValid)
            {

                // Check if the user already exists
                var existingUser = db.Staffs.FirstOrDefault(u => u.StaffEmail == s1.StaffEmail);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "Admin with this email already exists.";
                    return View(s1);
                }

                // Get the maximum Student_ID from the database
                var maxAdminId = db.Staffs.Max(u => (int?)u.StaffId) ?? 0;

                // Increment the maximum Student_ID by 1
                s1.StaffId = maxAdminId + 1;

                Staff sl = new Staff();
                string path = UploadImage(imgfile);
                if (path.Equals("-1"))
                {

                }
                else
                {
                    try
                    {
                        sl.StaffId = s1.StaffId;
                        sl.StaffName = s1.StaffName;
                        sl.StaffEmail = s1.StaffEmail;
                        sl.StaffAddress = s1.StaffAddress;
                        sl.StaffPhone = s1.StaffPhone;
                        sl.StaffImage = path;

                        db.Staffs.Add(sl);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Admin added successfully.";
                        return RedirectToAction("AddAdmin", "Admin"); // Redirect to a different action
                    }
                    catch (DbEntityValidationException ex)
                    {
                        // Handle validation errors
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }


            }

            // If ModelState is not valid, return the view with the invalid model
            return View(s1);
        }

        public ActionResult ViewAdmin()
        {
            var Admin = db.Staffs.ToList(); // Retrieve all students from the database
            return View(Admin);
        }


        public ActionResult EditAdmin(int id)
        {
            var Admin = db.Staffs.FirstOrDefault(s => s.StaffId == id);

            if (Admin == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Admin record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(Admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdmin(Staff editedAdmin)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingUser = db.Staffs.FirstOrDefault(s => s.StaffId == editedAdmin.StaffId);

                if (existingUser != null)
                {
                    // Update student information with edited values
                    existingUser.StaffId = editedAdmin.StaffId;
                    existingUser.StaffEmail = editedAdmin.StaffEmail;
                    existingUser.StaffPhone = editedAdmin.StaffPhone;
                    existingUser.StaffName = editedAdmin.StaffName;
                    existingUser.StaffAddress = editedAdmin.StaffAddress;

                    // Save changes to the database
                    db.SaveChanges();

                    // Display success message
                    TempData["SuccessMessage"] = "Admin information updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Admin record not found.";
                }
            }
            else
            {
                // If ModelState is not valid, return the view with the invalid model
                return View(editedAdmin);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("ViewAdmin", "Admin", new { id = editedAdmin.StaffId });
        }

        [HttpPost]
        public ActionResult DeleteAdmin(int id)
        {
            var Admin = db.Staffs.Find(id);
            if (Admin == null)
            {
                return HttpNotFound(); // or handle appropriately if student is not found
            }


            // Now delete the student record
            db.Staffs.Remove(Admin);
            db.SaveChanges();

            return RedirectToAction("ViewAdmin"); // Redirect back to the same page
        }

        [HttpGet]
        public ActionResult AddFaculty()
        {
            var newFaculty = new Faculty();

            // Get the maximum Student_ID from the database
            var maxFacultyId = db.Faculties.Max(u => (int?)u.FacultyId) ?? 0;

            // Set the Student_ID for the new student as the maximum ID + 1
            newFaculty.FacultyId = maxFacultyId + 1;

            return View(newFaculty);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFaculty(Faculty s1, HttpPostedFileBase imgfile)

        {
            if (ModelState.IsValid)
            {

                // Check if the user already exists
                var existingUser = db.Faculties.FirstOrDefault(u => u.FacultyEmail == s1.FacultyEmail);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "User with this email already exists.";
                    return View(s1);
                }

                // Get the maximum Student_ID from the database
                var maxFacultyId = db.Faculties.Max(u => (int?)u.FacultyId) ?? 0;

                // Increment the maximum Student_ID by 1
                s1.FacultyId = maxFacultyId + 1;

                Faculty sl = new Faculty();
                string path = UploadImage(imgfile);
                if (path.Equals("-1"))
                {

                }
                else
                {
                    try
                    {
                        sl.FacultyId = s1.FacultyId;
                        sl.FacultyName = s1.FacultyName;
                        sl.FacultyEmail = s1.FacultyEmail;
                        sl.FacultyAddress = s1.FacultyAddress;
                        sl.FacultyPhone = s1.FacultyPhone;
                        sl.FacultyImage = path;

                        db.Faculties.Add(sl);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Faculty added successfully.";
                        return RedirectToAction("ViewFaculty", "Admin"); // Redirect to a different action
                    }
                    catch (DbEntityValidationException ex)
                    {
                        // Handle validation errors
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }


            }

            // If ModelState is not valid, return the view with the invalid model
            return View(s1);
        }


        public ActionResult ViewFaculty()
        {
            var Faculty = db.Faculties.ToList(); // Retrieve all students from the database
            return View(Faculty);
        }

        public ActionResult EditFaculty(int id)
        {
            var student = db.Faculties.FirstOrDefault(s => s.FacultyId == id);

            if (student == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Faculty record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFaculty(Faculty editedFaculty)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingUser = db.Faculties.FirstOrDefault(s => s.FacultyId == editedFaculty.FacultyId);

                if (existingUser != null)
                {
                    existingUser.FacultyId = editedFaculty.FacultyId;
                    existingUser.FacultyEmail = editedFaculty.FacultyEmail;
                    existingUser.FacultyPhone = editedFaculty.FacultyPhone;
                    existingUser.FacultyName = editedFaculty.FacultyName;
                    existingUser.FacultyAddress = editedFaculty.FacultyAddress;

                    // Save changes to the database
                    db.SaveChanges();

                    // Display success message
                    TempData["SuccessMessage"] = "Admin information updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Admin record not found.";
                }
            }
            else
            {
                // If ModelState is not valid, return the view with the invalid model
                return View(editedFaculty);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("ViewFaculty", "Admin", new { id = editedFaculty.FacultyId });
        }

        [HttpPost]
        public ActionResult DeleteFaculty(int id)
        {
            var student = db.Faculties.Find(id);
            if (student == null)
            {
                return HttpNotFound(); // or handle appropriately if student is not found
            }

            // Now delete the student record
            db.Faculties.Remove(student);
            db.SaveChanges();

            return RedirectToAction("ViewFaculty"); // Redirect back to the same page
        }


        public ActionResult AdminMain(int id)
        {
            // Retrieve the student record using the provided ID
            var Admin = db.Staffs.FirstOrDefault(s => s.StaffId == id);
            int totalStudents = db.Students.Count();
            int totalUsers = db.Users.Count();
            int totalClasses = db.Classes.Count();

            // Pass the counts to the view using ViewBag
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalClasses = totalClasses;

            if (Admin != null)
            {
                // Pass the student model to the view
                return View(Admin);
            }
            else
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Admin record not found.";
                return RedirectToAction("Index");
            }
        }


        public string UploadImage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);

                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                        // Log the exception or handle it appropriately
                    }
                }
                else
                {

                    Response.Write("<script>alert('Only jpg, jpeg, or png formats are acceptable....');</script>");
                }
            }
            else
            {

                Response.Write("<script>alert('Please select a file');</script>");
            }

            return path;
        }
    }
}