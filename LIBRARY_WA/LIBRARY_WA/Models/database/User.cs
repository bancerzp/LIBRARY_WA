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
        public User(int user_id, string login, string password, string user_type, string fullname, DateTime date_of_birth, string phone_number, string email, string address, bool is_valid)
        {
            this.user_id = user_id;
            this.login = login;
            this.password = password;
            this.user_type = user_type;
            this.fullname = fullname;
            this.date_of_birth = date_of_birth;
            this.phone_number = phone_number;
            this.email = email;
            this.address = address;
            this.is_valid = is_valid;
        }

        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        
        public String login { get; set; }
        public String password { get; set; }
        public String user_type { get; set; }
        [MaxLength]
        public String fullname { get; set; }
        public DateTime date_of_birth { get; set; }
        public String phone_number { get; set; }
        [EmailAddress]
        public String email { get; set; }
        public String address { get; set; }
        public Boolean is_valid { get; set; }
    }
}
