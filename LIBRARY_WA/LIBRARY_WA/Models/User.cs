using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class User
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [MaxLength(10),MinLength(5)]
        public String login { get; set; }
        public String password { get; set; }
        [MaxLength(1)]
        public String user_type { get; set; }
        [MaxLength(50)]
        public String fullname { get; set; }
        public DateTime date_of_birth { get; set; }
        [StringLength(12)]
        public String phone_number { get; set; }
        [MaxLength(60)]
        public String email { get; set; }
        [MaxLength(100)]
        public String address { get; set; }
        public Boolean is_valid { get; set; }
    }
}
