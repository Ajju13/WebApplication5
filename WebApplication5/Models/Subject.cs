using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models
{
    public class Subject
    {
        public Subject()
        {
            Classes = new HashSet<Class>();
        }

        [Key]
        public long CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        public long? PreRequisiteCourseId { get; set; }

        public long CreditHours { get; set; }

        public virtual ICollection<Class> Classes { get; set; }

        [ForeignKey("PreRequisiteCourseId")]
        public virtual Subject PreRequisiteCourse { get; set; }
    }
}