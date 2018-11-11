using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class User
    {
        public String userId { get; set; }
        public String login { get; set; }
        public String password { get; set; }

        public String userType { get; set; }
        public String fullName { get; set; }
        public String date_of_birth { get; set; }
        public String phone_number { get; set; }
        public String email { get; set; }
        public String address { get; set; }
    }
}
