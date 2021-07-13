using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        [MaxLength]
        public string Fullname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsValid { get; set; }

        public User(int userId, string login, string password, string userType, string fullname, DateTime dateOfBirth, string phoneNumber, string email, string address, bool isValid)
        {
            UserId = userId;
            Login = login;
            Password = password;
            UserType = userType;
            Fullname = fullname;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            IsValid = isValid;
        }

        public User(string password, string userType, string fullname, DateTime dateOfBirth, string phoneNumber, string email, string address, bool isValid)
        {
            UserId = UserId;
            Login = Login;
            Password = password;
            UserType = userType;
            Fullname = fullname;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            IsValid = isValid;
        }
    }
}