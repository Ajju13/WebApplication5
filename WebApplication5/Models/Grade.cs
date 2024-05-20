using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using System.Web;

namespace WebApplication5.Models
{
    public class Grade
    {
        [Key]
        public long GradeId { get; set; }

        [Required]
        public long GradeValue { get; set; }

        [Required]
        public long MinMarks { get; set; }

        [Required]
        public long MaxMarks { get; set; }
    }
}