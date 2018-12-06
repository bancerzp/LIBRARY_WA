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
        }


        public List<String> GetEmail(String email)
        {
            return _context.User.Where(u => u.email == email).ToList();
        }

        //----Data verifying

        public IActionResult IfEmailExists([FromRoute] String email)
        {
           
            //    String email2 = email.Replace("'", "");

            return Ok();

        }


        public IEnumerable<User> IfLoginExists([FromRoute] String login)
        {
            String login2 = login.Replace("'", "");
            return _context.User.Where(u => u.login == login2);
        }


        public IActionResult IsLogged([FromBody] User userData)
        {
            if (userData == null)
            {
                return BadRequest(new { alert = "Nieprawidłowe dane logowania" });
            }
            User user = _context.User.Where(u => u.login == userData.login.Replace("'", "\'") && u.password == userData.password.Replace("'", "\'")).FirstOrDefault();
            if (user != null)

            {
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
                return Ok(new { Token = tokenString, id = user.user_id, user_type = user.user_type, fullname = user.fullname, expires = DateTime.Now.AddHours(5) });
            }
            else
            {
                return Unauthorized();
            }

        }



        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
           
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { alert = "Użytkownik o danym id nie istnieje!" });
            }

            return Ok(user);
        }




        public async Task<IActionResult> SearchUser([FromBody] String[] search)
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

            var user = _context.User.FromSql(sql);

            return Ok(user);
        }



        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid || user.login.Contains("'"))
            {
                return BadRequest(ModelState);
            }

            if (_context.User.Where(a => a.login == user.login).Count() > 0)
            {
                return BadRequest(new { alert = "Dany login jest już zajęty" });
            }

            if (_context.User.Where(a => a.email == user.email).Count() > 0)
            {
                return BadRequest(new { alert = "Dany email już istnieje w bazie danych." });
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("AddUser", new { id = user.user_id }, user);
        }


        public async Task<IActionResult> RemoveUser([FromRoute] int id)
        {

            //jeśli ma wypożyczone książki to komunikat, że nie można usunąć użytkownika bo ma niewyszystkie książki oddane, 
            //a jesli usunięty to zmienia isValid na false
          

            if (_context.Rent.Where(a => a.user_id == id).Count() > 0)
            {
                return Ok(new { alert = "Nie można usunąć użytkownika , bo ma nieoddane książki!!" });
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

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
            await _context.SaveChangesAsync();

            return Ok();
        }




        public IEnumerable<Rent> GetRent([FromRoute] int id)
        {
            return _context.Rent.Where(a => a.user_id == id);
        }

        public IEnumerable<Reservation> GetReservation([FromRoute] int id)
        {
            return _context.Reservation.Where(a => a.user_id == id);
        }


        public IEnumerable<Renth> GetRenth([FromRoute] int id)
        {
            return _context.Renth.Where(a => a.user_id == id);
        }


        public async Task<IActionResult> CancelReservation([FromRoute] int id)
        {
      
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { alert = "Nie istnieje rezerwacja o danym id!" });
            }
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
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }



        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.User.Update(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

    }
}