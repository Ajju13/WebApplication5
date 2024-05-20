using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models
{
    public class Class
    {
        public Class()
        {
            ClassEnrolments = new HashSet<ClassEnrolment>();
            ClassStdResults = new HashSet<Class_StdResult>();
            ClassTests = new HashSet<Class_Test>();
        }

        [Key]
        public long ClassId { get; set; }

        [ForeignKey("Subject")]
        public long CourseId { get; set; }

        [ForeignKey("Faculty")]
        public long FacultyId { get; set; }

        public string ClassDay { get; set; }
        public long ClassSession { get; set; }

        public virtual Subject Subject { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual ICollection<ClassEnrolment> ClassEnrolments { get; set; }
        public virtual ICollection<Class_StdResult> ClassStdResults { get; set; }
        public virtual ICollection<Class_Test> ClassTests { get; set; }
    }
}