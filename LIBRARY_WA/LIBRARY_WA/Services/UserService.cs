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
            return _context.User.Any(u => u.Email.Equals(email));
        }

        public bool IfLoginExists(string login)
        {
            return _context.User.Any(user => user.Login.Equals(login));
        }
        private User GetUserByLogin(string login)
        {
            var user = _context.User.FirstOrDefault(u => u.Login.Equals(login));

            if (user is null)
            {
                throw new Exception($"Użytkownik o loginie: '{login}' nie istnieje!");
            }

            return user;
        }

        public bool IsLoggedCheckData(UserDTO userData)
        {
            return _context.User.Any(user => IfLoginExists(user.Login) && user.Password.Equals(userData.Password));
        }

        public void ChangeUserStatus(StatusDTO status)
        {
            User us = _context.User.Where(user => user.UserId.Equals(status.UserId)).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.IsValid = status.Status;// (values[1]=="true");
            _context.SaveChanges();
        }

        public LoginInfo GetLoggedInfo(string login, string password)
        {
            User user = _context.User.Where(u => u.Login.Equals(login) && u.Password.Equals(password)).FirstOrDefault();

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
            var user = _context.User.Find(id);

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

            List<User> user_db = _context.User.FromSql(sql).ToList();
            List<UserDTO> user_dto = new List<UserDTO>();
            foreach (User userr in user_db)
            {
                user_dto.Add(new UserDTO(userr.UserId, userr.Login, userr.Password, userr.UserType, userr.Fullname, userr.DateOfBirth, userr.PhoneNumber, userr.Email, userr.Address, userr.IsValid));
            }

            return user_dto;
        }

        public string AddUserCheckData(UserDTO user)
        {
            if (_context.User.Where(a => a.Email == user.Email).Any())
            {
                return "Dany email już istnieje w bazie danych.";
            }
            return "";
        }

        public UserDTO AddUser(UserDTO user)
        {
            User new_user = new User(user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);

            _context.User.Add(new_user);
            _context.SaveChanges();
            new_user.Login = new_user.UserId.ToString().PadLeft(12, '0');
            _context.User.Update(new_user);
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

            if (_context.Rent.Where(a => a.UserId.Equals(id)).Any())
            {
                return "Nie można usunąć użytkownika , bo ma nieoddane książki!!";
            }
            return "";
        }

        public void RemoveUser(int id)
        {
            User user = _context.User.Find(id);
            var reservation = _context.Reservation.Where(a => a.UserId.Equals(id)).ToArray();
            var bookIds = reservation.Select(a => a.BookId).ToArray();

            Reservation[] r;
            foreach (int bookId in bookIds)
            {
                int usqueue = _context.Reservation.Where(a => a.UserId == id && a.BookId == bookId).First().Queue;
                r = _context.Reservation.Where(a => a.BookId == bookId && a.Queue > usqueue).ToArray();
                foreach (Reservation res in reservation)
                {
                    res.Queue = res.Queue - 1;
                }
                _context.Reservation.Remove(_context.Reservation.Where(a => a.UserId == id && a.BookId == bookId).First());
                _context.SaveChanges();
            }

            _context.User.Remove(user);
            // user.isValid = false;
            _context.SaveChanges();
        }

        public IEnumerable<RentDTO> GetRents(int userId)
        {
            List<Rent> rent_db = _context.Rent.Where(a => a.UserId == userId).ToList();
            List<RentDTO> RentDTO = new List<RentDTO>();
            Book book;
            foreach (Rent rent in rent_db)
            {
                book = _context.Book.Where(a => a.BookId == rent.BookId).FirstOrDefault();
                RentDTO.Add(new RentDTO(rent.RentId, rent.UserId, rent.BookId, book.Title, book.Isbn, rent.VolumeId, rent.StartDate, rent.ExpireDate));
            }
            return RentDTO;
        }

        public IEnumerable<ReservationDTO> GetReservations(int userId)
        {
            List<Reservation> reservation_db = _context.Reservation.Where(a => a.UserId == userId).ToList();
            var ReservationDTO = new List<ReservationDTO>();

            Book book;
            foreach (Reservation reservation in reservation_db)
            {
                book = _context.Book.Where(a => a.BookId.Equals(reservation.BookId)).FirstOrDefault();
                ReservationDTO.Add(new ReservationDTO(reservation.ReservationId, reservation.UserId, book.Title, book.Isbn, reservation.BookId, reservation.VolumeId, reservation.StartDate, reservation.ExpireDate, reservation.Queue, reservation.IsActive));
            }
            return ReservationDTO;
        }

        public IEnumerable<RenthDTO> GetRentsHistory(int userId)
        {
            List<Renth> renth_db = _context.Renth.Where(a => a.UserId == userId).ToList();
            var renth_dto = new List<RenthDTO>();

            Book book;
            foreach (Renth renth in renth_db)
            {
                book = _context.Book.Where(a => a.BookId.Equals(renth.BookId)).FirstOrDefault();
                renth_dto.Add(new RenthDTO(renth.RentIdH, renth.UserId, book.Title, book.Isbn, renth.BookId, renth.VolumeId, renth.StartDate, renth.EndDate));

            }
            return renth_dto;
        }

        public bool CancelReservationCheckData(int id)
        {
            return _context.Reservation.Where(reservation => reservation.ReservationId.Equals(id)).Any();
        }

        public void CancelReservation(int id)
        {
            Reservation reservation = _context.Reservation.Find(id);
            var resToChange = _context.Reservation.Where(a => a.BookId == reservation.BookId && a.Queue > reservation.Queue).ToArray();
            foreach (Reservation res in resToChange)
            {
                res.Queue--;
                if (res.Queue == 0)
                {
                    res.IsActive = true;
                    res.ExpireDate = DateTime.Now.AddMinutes(1);
                    res.VolumeId = reservation.VolumeId;
                }
            }

            _context.Reservation.Remove(reservation);
            _context.SaveChanges();
        }

        public void ResetPassword(UserDTO user)
        {
            // _context.User.Remove(_context.User.Where(a => a.userId == user.userId).FirstOrDefault());
            User us = _context.User.Where(a => a.UserId == user.UserId).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.Password = user.Password;
            _context.SaveChanges();
        }

        public void UpdateUser(UserDTO user)
        {
            _context.User.Remove(_context.User.Where(a => a.UserId == user.UserId).FirstOrDefault());
            User us = new User(user.UserId, user.Login, user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);
            //_context.Entry(us).State = EntityState.Modified;
            _context.User.Add(us);
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