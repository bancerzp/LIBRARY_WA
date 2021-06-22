using System;
using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models
{
    public class User_DTO
    {
        public User_DTO(int user_id, string login, string password, string user_type, string fullname, DateTime date_of_birth, string phone_number, string email, string address, bool is_valid)
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

        public int user_id { get; set; }
        [MinLength(5)]
        public string login { get; set; }
        [MinLength(1)]
        public string password { get; set; }
        public string user_type { get; set; }
        public string fullname { get; set; }
        public DateTime date_of_birth { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public bool is_valid { get; set; }
    }
}