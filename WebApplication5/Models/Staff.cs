using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Staff
    {
        [Key]
        public long StaffId { get; set; }

        [Required]
        [EmailAddress]
        public string StaffEmail { get; set; }

        [Required]
        [Phone]
        public string StaffPhone { get; set; }

        [Required]
        public string StaffName { get; set; }

        public string StaffAddress { get; set; }
        public string StaffImage { get; set; }
    }
}