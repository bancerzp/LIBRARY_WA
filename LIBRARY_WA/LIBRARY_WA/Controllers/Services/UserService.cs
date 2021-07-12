using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using LIBRARY_WA.Models.DTO;

namespace LIBRARY_WA.Controllers.Services
{

    public class UserService
    {
        public IConfiguration Configuration { get; }

        private readonly LibraryContext _context;

        public UserService(LibraryContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _context = context;
        }

        public bool IfEmailExists(string email)
        {
            return _context.User.Where(u => u.Email == email).Count() > 0;
        }

        public bool IfLoginExists(string login)
        {
            string login2 = login.Replace("'", "");
            return _context.User.Where(u => u.Login == login2).Count() > 0;
        }

        public bool IsLoggedCheckData(User_DTO userData)
        {
            return _context.User.Where(u => u.Login == userData.Login.Replace("'", "\'") && u.Password == userData.Password.Replace("'", "\'") ).Count() > 0;
        }

        public bool IsBlocked(User_DTO userData)
        {
            return _context.User.Where(a => a.Login == userData.Login).SingleOrDefault().IsValid;
        }

        public void ChangeUserStatus(Status_DTO status)
        {
            User us = _context.User.Where(a => a.UserId == status.UserId).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.IsValid = status.Status;// (values[1]=="true");
            _context.SaveChanges();
        }

        public (string Token, int id, string userType, string fullname, DateTime expires) IsLogged(User_DTO userData)
        {
            User user = _context.User.Where(u => u.Login == userData.Login.Replace("'", "\'") && u.Password == userData.Password.Replace("'", "\'")).FirstOrDefault();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                     new Claim(ClaimTypes.Name, user.Fullname),
                     new Claim(ClaimTypes.Role, user.UserType),
                     new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5001",
                audience: "http://localhost:5001",
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return (Token: tokenString, id: user.UserId, userType: user.UserType, fullname: user.Fullname, expires: DateTime.Now.AddHours(5));
        }



        public User_DTO GetUserById(int id)
        {

            var user = _context.User.Find(id);

            if (user == null)
            {
                return null;
            }

            return new User_DTO(user.UserId, user.Login, user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);
        }


        public List<User_DTO> SearchUser(String[] search)
        {
            String[] name = { "userId", "fullname", "email", "login", "phoneNumber" };
            string sql = "Select * from User where 1=1 ";
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
            List<User_DTO> user_dto = new List<User_DTO>();
            foreach (User userr in user_db)
            {
                user_dto.Add(new User_DTO(userr.UserId, userr.Login, userr.Password, userr.UserType, userr.Fullname, userr.DateOfBirth, userr.PhoneNumber, userr.Email, userr.Address, userr.IsValid));
            }

            return user_dto;
        }

        public string AddUserCheckData(User_DTO user)
        {
            if (_context.User.Where(a => a.Email == user.Email).Count() > 0)
            {
                return "Dany email już istnieje w bazie danych.";
            }
            return "";
        }

        public User_DTO AddUser(User_DTO user)
        {
            User new_user= new User(user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);
           
            _context.User.Add(new_user);
            _context.SaveChanges();
            new_user.Login = new_user.UserId.ToString().PadLeft(12, '0');
             _context.User.Update(new_user);
            _context.SaveChanges();
            return new User_DTO(new_user.UserId, new_user.Login, new_user.Password, new_user.UserType, new_user.Fullname, new_user.DateOfBirth, new_user.PhoneNumber, new_user.Email, new_user.Address, new_user.IsValid);
        }

        public string RemoveUserCheckData(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                return ("Użytkownik nie istnieje!");
            }

            if (_context.Rent.Where(a => a.UserId == id).Count() > 0)
            {
                return "Nie można usunąć użytkownika , bo ma nieoddane książki!!";
            }
            return "";
        }

        public void RemoveUser(int id)
        {
            User user = _context.User.Find(id);
            Reservation[] reservation = _context.Reservation.Where(a => a.UserId == id).ToArray();
            int[] bookIds = reservation.Select(a => a.BookId).ToArray();

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

        public IEnumerable<Rent_DTO> GetRents(int userId)
        {

            List<Rent> rent_db = _context.Rent.Where(a => a.UserId == userId).ToList();
            List<Rent_DTO> rent_dto = new List<Rent_DTO>();
            Book book;
            foreach (Rent rent in rent_db)
            {
                book = _context.Book.Where(a => a.BookId == rent.BookId).FirstOrDefault();
                rent_dto.Add(new Rent_DTO(rent.RentId, rent.UserId, rent.BookId, book.Title, book.Isbn, rent.VolumeId, rent.StartDate, rent.ExpireDate));
            }
            return rent_dto;
        }

        public IEnumerable<Reservation_DTO> GetReservations(int userId)
        {
            List<Reservation> reservation_db = _context.Reservation.Where(a => a.UserId == userId).ToList();
            List<Reservation_DTO> reservation_dto = new List<Reservation_DTO>();

            Book book;
            foreach (Reservation reservation in reservation_db)
            {
                book = _context.Book.Where(a => a.BookId == reservation.BookId).FirstOrDefault();
                reservation_dto.Add(new Reservation_DTO(reservation.ReservationId, reservation.UserId, book.Title, book.Isbn, reservation.BookId, reservation.VolumeId, reservation.StartDate, reservation.ExpireDate, reservation.Queue, reservation.IsActive));
            }
            return reservation_dto;

        }


        public IEnumerable<Renth_DTO> GetRentsHistory(int userId)
        {
            List<Renth> renth_db = _context.Renth.Where(a => a.UserId == userId).ToList();
            List<Renth_DTO> renth_dto = new List<Renth_DTO>();

            Book book;
            foreach (Renth renth in renth_db)
            {
                book = _context.Book.Where(a => a.BookId == renth.BookId).FirstOrDefault();
                renth_dto.Add(new Renth_DTO(renth.RentIdH, renth.UserId, book.Title, book.Isbn, renth.BookId, renth.VolumeId, renth.StartDate, renth.EndDate));

            }
            return renth_dto;

        }

        public bool CancelReservationCheckData(int id)
        {

            return _context.Reservation.Where(a => a.ReservationId == id).Count() > 0;
        }

        public void CancelReservation(int id)
        {

            Reservation reservation = _context.Reservation.Find(id);
            Reservation[] resToChange = _context.Reservation.Where(a => a.BookId == reservation.BookId && a.Queue > reservation.Queue).ToArray();
            foreach (Reservation res in resToChange)
            {
                res.Queue = res.Queue - 1;
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


        public void ResetPassword(User_DTO user) {
           // _context.User.Remove(_context.User.Where(a => a.userId == user.userId).FirstOrDefault());
            User us = _context.User.Where(a => a.UserId == user.UserId).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.Password=user.Password;
            _context.SaveChanges();
        }


        public void UpdateUser(User_DTO user)
        {
            _context.User.Remove(_context.User.Where(a => a.UserId == user.UserId).FirstOrDefault());
            User us = new User(user.UserId, user.Login, user.Password, user.UserType, user.Fullname, user.DateOfBirth, user.PhoneNumber, user.Email, user.Address, user.IsValid);
            //_context.Entry(us).State = EntityState.Modified;
            _context.User.Add(us);
            _context.SaveChanges();
        }
    }
}