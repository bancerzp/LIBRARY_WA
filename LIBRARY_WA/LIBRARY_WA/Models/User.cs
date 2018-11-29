using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class User
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public String login { get; set; }
        public String password { get; set; }
        public String user_Type { get; set; }
        [MaxLength]
        public String fullname { get; set; }
        public DateTime date_of_birth { get; set; }
        public String phone_number { get; set; }
        public String email { get; set; }
        public String address { get; set; }
        public Boolean is_valid { get; set; }
    }
}
