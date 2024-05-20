using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebApplication5.Models
{
    public class Student
    {
        public Student()
        {
            ClassEnrolments = new HashSet<ClassEnrolment>();
            ClassStdResults = new HashSet<Class_StdResult>();
        }

        [Key]
        public long StudentId { get; set; }

        [Required(ErrorMessage = "Student Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string StudentEmail { get; set; }

        [Required]
        [Phone]
        public string StudentPhone { get; set; }

        [Required(ErrorMessage = "Student Name is required")]
        public string StudentName { get; set; }

        public string StudentAddress { get; set; }
        public string StudentImage { get; set; }


        public ICollection<ClassEnrolment> ClassEnrolments { get; set; }
        public ICollection<Class_StdResult> ClassStdResults { get; set; }
    }
}