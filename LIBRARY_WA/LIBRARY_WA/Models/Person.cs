using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Person
    {
        [Required]
        public String personId { get; set; }
        public String first_name { get; set; }
        public String last_name { get; set; }
        public String date_of_birth { get; set; }
        public String phone_number { get; set; }
        public String email { get; set; }
        public String pesel { get; set; }
    }
}
