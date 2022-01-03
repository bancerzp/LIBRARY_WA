using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Library.Models.DTO;

namespace Library.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly LibraryContext _context;

        public UserService(LibraryContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public bool IfEmailExists(string email)
        {
            return _context.Users.Any(u => u.Email.Equals(email));
        }

        public bool IfLoginExists(string login)
        {
            return _context.Users.Any(user => user.Login.Equals(login));
        }

        private User GetUserByLogin(string login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login.Equals(login));

            if (user is null)
            {
                throw new Exception($"Użytkownik o loginie: '{login}' nie istnieje!");
            }

            return user;
        }

        public bool IsLoggedCheckData(UserDTO userData)
        {
            return _context.Users.Any(user => IfLoginExists(user.Login) && user.Password.Equals(userData.Password));
        }

        public void ChangeUserStatus(StatusDTO status)
        {
            User us = _context.Users.Where(user => user.UserId.Equals(status.UserId)).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.IsValid = status.Status;// (values[1]=="true");
            _context.SaveChanges();
        }

        public LoginInfo GetLoggedInfo(string login, string password)
        {
            User user = _context.Users.Where(u => u.Login.Equals(login) && u.Password.Equals(password)).FirstOrDefault();

            if (user is null)
                return null;

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                     new Claim(ClaimTypes.Name, user.Fullname),
                     new Claim(ClaimTypes.Role, user.UserType),
                     new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5001/",
                audience: "http://localhost:5001/",
                claims: claims,
                expires: DateTime.Now.AddHours(DefaultValues.TokenExpirationTimeInHours),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new LoginInfo()
            {
                Token = tokenString,
                Id = user.UserId,
                UserType = user.UserType,
                Fullname = user.Fullname,
                Expires = DateTime.Now.AddHours(DefaultValues.TokenExpirationTimeInHours)
            };
        }

        public UserDTO GetUserById(int id)
        {
            var user = _context.Users.Find(id);

            if (user is null)
            {
                return null;
            }

            return new UserDTO(user.UserId, user.Login, user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);
        }

        public List<UserDTO> SearchUser(string[] search)
        {
            string[] name = { "userId", "fullname", "email", "login", "phoneNumber" };
            string sql = "Select * from [dbo].[User]";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    if (name[i] == "fullname")
                    {
                        sql += "and " + name[i] + " like('%" + search[i].Replace("'", "\'") + "%') ";
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i].Replace("'", "\'") + "' ";
                    }
                }
            }

            List<User> user_db = _context.Users.FromSql(sql).ToList();
            List<UserDTO> user_dto = new List<UserDTO>();
            foreach (User userr in user_db)
            {
                user_dto.Add(new UserDTO(userr.UserId, userr.Login, userr.Password, userr.UserType, userr.Fullname, userr.DateOfBirth, userr.PhoneNumber, userr.Email, userr.Address, userr.IsValid));
            }

            return user_dto;
        }

        public string AddUserCheckData(UserDTO user)
        {
            if (_context.Users.Where(a => a.Email == user.Email).Any())
            {
                return "Dany email już istnieje w bazie danych.";
            }
            return "";
        }

        public UserDTO AddUser(UserDTO user)
        {
            User new_user = new User(user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);

            _context.Users.Add(new_user);
            _context.SaveChanges();
            new_user.Login = new_user.UserId.ToString().PadLeft(12, '0');
            _context.Users.Update(new_user);
            _context.SaveChanges();
            return new UserDTO(new_user.UserId, new_user.Login, new_user.Password, new_user.UserType, new_user.Fullname, new_user.DateOfBirth, new_user.PhoneNumber, new_user.Email, new_user.Address, new_user.IsValid);
        }

        public string RemoveUserCheckData(int id)
        {
            var user = GetUserById(id);
            if (user is null)
            {
                return ("Użytkownik nie istnieje!");
            }

            return "";
        }

        public void RemoveUser(int id)
        {
            User user = _context.Users.Find(id);
            _context.Users.Remove(user);
            // user.isValid = false;
            _context.SaveChanges();
        }

        public void ResetPassword(UserDTO user)
        {
            // _context.User.Remove(_context.User.Where(a => a.userId == user.userId).FirstOrDefault());
            User us = _context.Users.Where(a => a.UserId == user.UserId).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.Password = user.Password;
            _context.SaveChanges();
        }

        public void UpdateUser(UserDTO user)
        {
            _context.Users.Remove(_context.Users.Where(a => a.UserId == user.UserId).FirstOrDefault());
            User us = new User(user.UserId, user.Login, user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);
            //_context.Entry(us).State = EntityState.Modified;
            _context.Users.Add(us);
            _context.SaveChanges();
        }
    }

    public class LoginInfo
    {
        public string Token { get; set; }
        public int Id { get; set; }
        public string UserType { get; set; }
        public string Fullname { get; set; }
        public DateTime Expires { get; set; }
    }
}