using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace WebApplication5.Models
{
    public class Faculty
    {
        public Faculty()
        {
            Classes = new HashSet<Class>();
        }

        [Key]
        public long FacultyId { get; set; }

        [Required]
        [EmailAddress]
        public string FacultyEmail { get; set; }

        [Required]
        [Phone]
        public string FacultyPhone { get; set; }

        [Required]
        public string FacultyName { get; set; }

        public string FacultyAddress { get; set; }
        public string FacultyImage { get; set; }

        public ICollection<Class> Classes { get; set; }

    }
}