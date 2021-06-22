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
            return _context.User.Where(u => u.email == email).Count() > 0;
        }

        public bool IfLoginExists(string login)
        {
            string login2 = login.Replace("'", "");
            return _context.User.Where(u => u.login == login2).Count() > 0;
        }

        public bool IsLoggedCheckData(User_DTO userData)
        {
            return _context.User.Where(u => u.login == userData.login.Replace("'", "\'") && u.password == userData.password.Replace("'", "\'") ).Count() > 0;
        }

        public bool IsBlocked(User_DTO userData)
        {
            return _context.User.Where(a => a.login == userData.login).SingleOrDefault().is_valid;
        }

        public void ChangeUserStatus(Status_DTO status)
        {
            User us = _context.User.Where(a => a.user_id == status.user_id).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.is_valid = status.status;// (values[1]=="true");
            _context.SaveChanges();
        }

        public (string Token, int id, string user_type, string fullname, DateTime expires) IsLogged(User_DTO userData)
        {
            User user = _context.User.Where(u => u.login == userData.login.Replace("'", "\'") && u.password == userData.password.Replace("'", "\'")).FirstOrDefault();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                     new Claim(ClaimTypes.Name, user.fullname),
                     new Claim(ClaimTypes.Role, user.user_type),
                     new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString())
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return (Token: tokenString, id: user.user_id, user_type: user.user_type, fullname: user.fullname, expires: DateTime.Now.AddHours(5));
        }



        public User_DTO GetUserById(int id)
        {

            var user = _context.User.Find(id);

            if (user == null)
            {
                return null;
            }

            return new User_DTO(user.user_id, user.login, user.password, user.user_type, user.fullname, user.date_of_birth, user.phone_number, user.email, user.address, user.is_valid);
        }


        public List<User_DTO> SearchUser(String[] search)
        {
            String[] name = { "user_id", "fullname", "email", "login", "phone_number" };
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
                user_dto.Add(new User_DTO(userr.user_id, userr.login, userr.password, userr.user_type, userr.fullname, userr.date_of_birth, userr.phone_number, userr.email, userr.address, userr.is_valid));
            }

            return user_dto;
        }

        public string AddUserCheckData(User_DTO user)
        {
            if (_context.User.Where(a => a.email == user.email).Count() > 0)
            {
                return "Dany email już istnieje w bazie danych.";
            }
            return "";
        }

        public User_DTO AddUser(User_DTO user)
        {
            User new_user= new User(user.password, user.user_type, user.fullname, user.date_of_birth, user.phone_number, user.email, user.address, user.is_valid);
           
            _context.User.Add(new_user);
            _context.SaveChanges();
            new_user.login = new_user.user_id.ToString().PadLeft(12, '0');
             _context.User.Update(new_user);
            _context.SaveChanges();
            return new User_DTO(new_user.user_id, new_user.login, new_user.password, new_user.user_type, new_user.fullname, new_user.date_of_birth, new_user.phone_number, new_user.email, new_user.address, new_user.is_valid);
        }

        public string RemoveUserCheckData(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                return ("Użytkownik nie istnieje!");
            }

            if (_context.Rent.Where(a => a.user_id == id).Count() > 0)
            {
                return "Nie można usunąć użytkownika , bo ma nieoddane książki!!";
            }
            return "";
        }

        public void RemoveUser(int id)
        {
            User user = _context.User.Find(id);
            Reservation[] reservation = _context.Reservation.Where(a => a.user_id == id).ToArray();
            int[] bookId = reservation.Select(a => a.book_id).ToArray();

            Reservation[] r;
            foreach (int book_id in bookId)
            {
                int usqueue = _context.Reservation.Where(a => a.user_id == id && a.book_id == book_id).First().queue;
                r = _context.Reservation.Where(a => a.book_id == book_id && a.queue > usqueue).ToArray();
                foreach (Reservation res in reservation)
                {
                    res.queue = res.queue - 1;
                }
                _context.Reservation.Remove(_context.Reservation.Where(a => a.user_id == id && a.book_id == book_id).First());
                _context.SaveChanges();
            }

            _context.User.Remove(user);
           // user.is_valid = false;
            _context.SaveChanges();
        }

        public IEnumerable<Rent_DTO> GetRent(int id)
        {

            List<Rent> rent_db = _context.Rent.Where(a => a.user_id == id).ToList();
            List<Rent_DTO> rent_dto = new List<Rent_DTO>();
            Book book;
            foreach (Rent rent in rent_db)
            {
                book = _context.Book.Where(a => a.book_id == rent.book_id).FirstOrDefault();
                rent_dto.Add(new Rent_DTO(rent.rent_id, rent.user_id, rent.book_id, book.title, book.isbn, rent.volume_id, rent.start_date, rent.expire_date));
            }
            return rent_dto;
        }

        public IEnumerable<Reservation_DTO> GetReservation(int id)
        {
            List<Reservation> reservation_db = _context.Reservation.Where(a => a.user_id == id).ToList();
            List<Reservation_DTO> reservation_dto = new List<Reservation_DTO>();

            Book book;
            foreach (Reservation reservation in reservation_db)
            {
                book = _context.Book.Where(a => a.book_id == reservation.book_id).FirstOrDefault();
                reservation_dto.Add(new Reservation_DTO(reservation.reservation_id, reservation.user_id, book.title, book.isbn, reservation.book_id, reservation.volume_id, reservation.start_date, reservation.expire_date, reservation.queue, reservation.is_active));
            }
            return reservation_dto;

        }


        public IEnumerable<Renth_DTO> GetRenth(int id)
        {
            List<Renth> renth_db = _context.Renth.Where(a => a.user_id == id).ToList();
            List<Renth_DTO> renth_dto = new List<Renth_DTO>();

            Book book;
            foreach (Renth renth in renth_db)
            {
                book = _context.Book.Where(a => a.book_id == renth.book_id).FirstOrDefault();
                renth_dto.Add(new Renth_DTO(renth.rent_id_h, renth.user_id, book.title, book.isbn, renth.book_id, renth.volume_id, renth.start_date, renth.end_date));

            }
            return renth_dto;

        }

        public bool CancelReservationCheckData(int id)
        {

            return _context.Reservation.Where(a => a.reservation_id == id).Count() > 0;
        }

        public void CancelReservation(int id)
        {

            Reservation reservation = _context.Reservation.Find(id);
            Reservation[] resToChange = _context.Reservation.Where(a => a.book_id == reservation.book_id && a.queue > reservation.queue).ToArray();
            foreach (Reservation res in resToChange)
            {
                res.queue = res.queue - 1;
                if (res.queue == 0)
                {
                    res.is_active = true;
                    res.expire_date = DateTime.Now.AddMinutes(1);
                    res.volume_id = reservation.volume_id;
                }
            }

            _context.Reservation.Remove(reservation);
            _context.SaveChanges();
        }


        public void ResetPassword(User_DTO user) {
           // _context.User.Remove(_context.User.Where(a => a.user_id == user.user_id).FirstOrDefault());
            User us = _context.User.Where(a => a.user_id == user.user_id).SingleOrDefault();
            _context.Entry(us).State = EntityState.Modified;
            us.password=user.password;
            _context.SaveChanges();
        }


        public void UpdateUser(User_DTO user)
        {
            _context.User.Remove(_context.User.Where(a => a.user_id == user.user_id).FirstOrDefault());
            User us = new User(user.user_id, user.login, user.password, user.user_type, user.fullname, user.date_of_birth, user.phone_number, user.email, user.address, user.is_valid);
            //_context.Entry(us).State = EntityState.Modified;
            _context.User.Add(us);
            _context.SaveChanges();
        }
    }
}