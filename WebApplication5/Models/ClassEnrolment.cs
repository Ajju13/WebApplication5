using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class ClassEnrolment
    {
        [Key]
        public long ClassEnrolmentId { get; set; }

        [ForeignKey("Class")]
        public long ClassId { get; set; }

        [ForeignKey("Student")]
        public long StudentId { get; set; }

        public Class Class { get; set; }
        public Student Student { get; set; }
    }
}