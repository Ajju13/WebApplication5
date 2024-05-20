using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Class_StdResult
    {
        [Key]
        public long CSRId { get; set; }

        [ForeignKey("Class")]
        public long ClassId { get; set; }

        [ForeignKey("Student")]
        public long StudentId { get; set; }

        public long Marks { get; set; }

        public Class Class { get; set; }
        public Student Student { get; set; }
    }
}