using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class TestType
    {
        public TestType()
        {
            ClassTests = new HashSet<Class_Test>();
        }

        [Key]
        public long TestTypeId { get; set; }

        [Required]
        public string TestTypeName { get; set; }

        public ICollection<Class_Test> ClassTests { get; set; }
    }
}