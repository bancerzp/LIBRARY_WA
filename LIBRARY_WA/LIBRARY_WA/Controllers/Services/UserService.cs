using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IO;
using AutoMapper;

namespace LIBRARY_WA.Controllers.Services
{

    public class UserService
    {
        public IConfiguration Configuration { get; }
        
        private readonly LibraryContext _context;
       
        public UserService(LibraryContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            this._context = context;
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, User_DTO>();
                cfg.CreateMap<User_DTO, User>();

            });
                
             
               
            }


        public Boolean IfEmailExists(String email)
        {
            return _context.User.Where(u => u.email == email).Count() > 0;
        }

        public Boolean IfLoginExists(String login)
        {
            String login2 = login.Replace("'", "");
            return _context.User.Where(u => u.login == login2).Count() > 0;
        }

        public Boolean IsLoggedCheckData(User_DTO userData)
        {
            return _context.User.Where(u => u.login == userData.login.Replace("'", "\'") && u.password == userData.password.Replace("'", "\'")).Count() > 0;
        }

        public (String Token, int id, String user_type, String fullname, DateTime expires) IsLogged(User_DTO userData)
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
          
            return Mapper.Map<User_DTO>(user);
        }


        public List<User_DTO> SearchUser(String[] search)
        {
            String[] name = { "user_id", "fullname", "email", "login", "phone_number" };
            String sql = "Select * from User where 1=1 ";
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
                user_dto.Add(Mapper.Map<User_DTO>(userr));
            }

            return user_dto;
        }

        public String AddUserCheckData(User_DTO user)
        {
            if (_context.User.Where(a => a.login == user.login).Count() > 0)
            {
                return "Dany login jest już zajęty";
            }

            if (_context.User.Where(a => a.email == user.email).Count() > 0)
            {
                return "Dany email już istnieje w bazie danych.";
            }
            return "";
        }

        public User_DTO AddUser(User_DTO user)
        {
           
            _context.User.Add(Mapper.Map<User>(user));
            _context.SaveChanges();
            return Mapper.Map<User_DTO>(user);
        }

        public String RemoveUserCheckData(int id)
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

            foreach (int book_id in bookId)
            {
                int usqueue = _context.Reservation.Where(a => a.user_id == id && a.book_id == book_id).First().queue;
                Reservation[] r = _context.Reservation.Where(a => a.book_id == book_id && a.queue > usqueue).ToArray();
                foreach (Reservation res in reservation)
                {
                    res.queue = res.queue - 1;
                }
                _context.Reservation.Remove(_context.Reservation.Where(a => a.user_id == id && a.book_id == book_id).First());
                _context.SaveChanges();
            }

            _context.User.Remove(user);
            user.is_valid = false;
            _context.SaveChanges();

        }
        
        public IEnumerable<Rent_DTO> GetRent(int id)
        {

            List<Rent> rent_db = _context.Rent.Where(a => a.user_id == id).ToList();
            List<Rent_DTO> rent_dto = new List<Rent_DTO>();
          
            foreach (Rent rent in rent_db)
            {
                Book book = _context.Book.Where(a => a.book_id == rent.book_id).FirstOrDefault(); 
                rent_dto.Add(new Rent_DTO(rent.rent_id, rent.user_id, rent.book_id,book.title,book.isbn,rent.volume_id,rent.start_date,rent.expire_date));
            }
            return rent_dto;
        }

        public IEnumerable<Reservation_DTO> GetReservation(int id)
        {
            List<Reservation> reservation_db = _context.Reservation.Where(a => a.user_id == id).ToList();
            List<Reservation_DTO> reservation_dto = new List<Reservation_DTO>();
           
            foreach (Reservation reservation in reservation_db)
            {
                Book book = _context.Book.Where(a => a.book_id == reservation.book_id).FirstOrDefault();
                reservation_dto.Add(new Reservation_DTO(reservation.reservation_id,reservation.user_id, book.title, book.isbn, reservation.book_id, reservation.volume_id, reservation.start_date, reservation.expire_date,reservation.queue,reservation.is_active));
            }
            return reservation_dto;

        }


        public IEnumerable<Renth_DTO> GetRenth(int id)
        {
            List<Renth> renth_db = _context.Renth.Where(a => a.user_id == id).ToList();
            List<Renth_DTO> renth_dto = new List<Renth_DTO>();
            
            foreach (Renth renth in renth_db)
            {
                Book book = _context.Book.Where(a => a.book_id == renth.book_id).FirstOrDefault();
                renth_dto.Add(new Renth_DTO(renth.rent_id_h,renth.user_id, book.title, book.isbn, renth.book_id, renth.volume_id, renth.start_date, renth.end_date));

            }
            return renth_dto;

        }

        public Boolean CancelReservationCheckData(int id)
        {
           
            return _context.Reservation.Where(a=>a.reservation_id==id).Count()>0;
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



        public void UpdateUser(User_DTO user)
        {
            User us = Mapper.Map<User>(user);
            _context.Entry(us).State = EntityState.Modified;
            _context.User.Update(us);
            _context.SaveChanges();

        }
    }
}