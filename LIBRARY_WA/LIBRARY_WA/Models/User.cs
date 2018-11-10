using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class User
    {
        [Required]
        public String personId { get; set; }
        [Required(ErrorMessage = "Pole login nie może być puste")]
        public String login { get; set; }

        [Required(ErrorMessage = "Pole hasło nie może być puste")]
        [DataType(DataType.Password)]
        public String password { get; set; }
        public String pesel { get; set; }

        public String userType { get; set; }
        public Person person { get; set; }
    }
}
