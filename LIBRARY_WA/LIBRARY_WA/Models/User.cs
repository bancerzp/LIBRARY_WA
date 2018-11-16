using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class User
    {
        [Key]
        public Int32 user_Id { get; set; }
        public String login { get; set; }
        public String password { get; set; }
        public String user_Type { get; set; }
        public String fullName { get; set; }
        public DateTime date_of_birth { get; set; }
        public String phone_number { get; set; }
        public String email { get; set; }
        public String address { get; set; }
        public Boolean is_valid { get; set; }
    }
}
