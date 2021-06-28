using System;
using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models
{
    public class User_DTO
    {
        public int UserId { get; set; }
        [MinLength(5)]
        public string Login { get; set; }
        [MinLength(1)]
        public string Password { get; set; }
        public string UserType { get; set; }
        public string Fullname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsValid { get; set; }

        public User_DTO(int userId, string login, string password, string userType, string fullname, DateTime dateOfBirth, string phoneNumber, string email, string address, bool isValid)
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
    }
}