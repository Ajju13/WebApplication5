using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;
using System.Web.Security;

namespace WebApplication5.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User Log)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == Log.Email && x.Password == Log.Password);

            if (user != null)
            {
                // Create authentication ticket
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,                                    // Ticket version
                    user.Email,                           // Username
                    DateTime.Now,                         // Issue date
                    DateTime.Now.AddMinutes(30),          // Expiration date
                    false,                                // Persistent
                    user.UserType,                       // User data (store user type)
                    FormsAuthentication.FormsCookiePath   // Cookie path
                );

                // Encrypt the ticket
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                // Create the cookie
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);

                // Redirect based on user type
                if (user.UserType == "S")
                {
                    var student = db.Students.FirstOrDefault(s => s.StudentEmail == Log.Email);
                    if (student != null)
                    {
                        return RedirectToAction("StudentMain", "Student", new { id = student.StudentId });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Student record not found.";
                        return View();
                    }
                }
                else if (user.UserType == "T")
                {
                    var teacher = db.Faculties.FirstOrDefault(t => t.FacultyEmail == Log.Email);
                    if (teacher != null)
                    {
                        return RedirectToAction("FacultyMain", "Faculty", new { id = teacher.FacultyId });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Teacher record not found.";
                        return View();
                    }
                }
                else if (user.UserType == "A")
                {
                    var admin = db.Staffs.FirstOrDefault(a => a.StaffEmail == Log.Email);
                    if (admin != null)
                    {
                        return RedirectToAction("AdminMain", "Admin", new { id = admin.StaffId });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Admin record not found.";
                        return View();
                    }
                }
            }

            TempData["ErrorMessage"] = "Incorrect email or password. Please try again.";
            return View();
        }



        public ActionResult Logout()
        {
            // Delete the authentication cookie
            FormsAuthentication.SignOut();

            // Redirect to the login page
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult CheckType()
        {
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
                    case "T":
                        var teacher = db.Faculties.FirstOrDefault(t => t.FacultyEmail == ticket.Name);
                        if (teacher != null)
                        {
                            userId = teacher.FacultyId;
                        }
                        break;
                    case "A":
                        var admin = db.Staffs.FirstOrDefault(a => a.StaffEmail == ticket.Name);
                        if (admin != null)
                        {
                            userId = admin.StaffId;
                        }
                        break;
                }

                // Redirect to the appropriate dashboard based on the user's type and retrieved ID
                switch (userType)
                {
                    case "S":
                        return RedirectToAction("StudentMain", "Student", new { id = userId });
                    case "T":
                        return RedirectToAction("FacultyMain", "Faculty", new { id = userId });
                    case "A":
                        return RedirectToAction("AdminMain", "Admin", new { id = userId });
                    default:
                        return RedirectToAction("Unauthorized", "Home"); // Redirect to unauthorized page if user type is unknown
                }
            }

            // If authentication cookie is not found, redirect to unauthorized page or handle accordingly
            return RedirectToAction("Unauthorized", "Home");
        }
    }
}