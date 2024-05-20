using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Class_Test
    {
        [Key]
        public long CTId { get; set; }

        [ForeignKey("Class")]
        public long ClassId { get; set; }

        [ForeignKey("TestType")]
        public long TestTypeId { get; set; }

        public string TestDate { get; set; }
        public long MaxMarks { get; set; }

        public Class Class { get; set; }
        public TestType TestType { get; set; }
    }
}