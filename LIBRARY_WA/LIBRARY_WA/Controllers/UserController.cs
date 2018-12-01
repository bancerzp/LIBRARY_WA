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

namespace LIBRARY_WA.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; }

       
        private readonly LibraryContext _context;

        public UserController(LibraryContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            this._context = context;
        }


        //----Data verifying
        [HttpGet("{email}")]
        public IEnumerable<User> IfEmailExists([FromRoute] String email)
        {
            String email2 = email.Replace("'", "");
            return _context.User.Where(u => u.email == email2);
        }

        [HttpGet("{login}")]
        public IEnumerable<User> IfLoginExists([FromRoute] String login)
        {
            String login2 = login.Replace("'", "");
            return _context.User.Where(u => u.login == login2);
        }


        // GET: api/User
        [HttpPost]
        public IActionResult IsLogged([FromBody] User userData)
        {
            if (userData == null)
            {
                return BadRequest(new { alert = "Nieprawidłowe zapytanie" });
            }
            User user = _context.User.Where(u => u.login == userData.login && u.password == userData.password).FirstOrDefault();
            if (user != null)

            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
        {
                 new Claim(ClaimTypes.Name, user.fullname),
                 new Claim(ClaimTypes.Role, user.user_type),
        };

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: claims,
                    expires: DateTime.Now.AddHours(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString, id = user.user_id, user_type = user.user_type, fullname = user.fullname,expires=DateTime.Now.AddHours(5) });
            }
            else
            {
                return Unauthorized();
            }

        }


        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { alert = "Użytkownik o danym id nie istnieje!" });
            }

            return Ok(user);
        }



        [HttpPost, Authorize(Roles = "l")]
        public async Task<IActionResult> SearchUser([FromBody] String[] search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            String[] name = { "user_id", "fullname", "email", "login", "phone_number" };
            String sql = "Select * from User where 1=1 ";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    if (name[i] == "fullname")
                    {
                        sql += "and " + name[i] + " like('%" + search[i] + "%') ";
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i] + "' ";
                    }
                }
            }

            var user =  _context.User.FromSql(sql);

            return Ok(user);
        }

      
        [HttpPost,Authorize(Roles ="l")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser([FromRoute] int id)
        {

            //jeśli ma wypożyczone książki to komunikat, że nie można usunąć użytkownika bo ma niewyszystkie książki oddane, 
            //a jesli usunięty to zmienia isValid na false
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            foreach(int book_id in bookId)
            {
                int usqueue = _context.Reservation.Where(a => a.user_id == id && a.book_id == book_id).First().queue;
                Reservation[] r = _context.Reservation.Where(a => a.book_id == book_id && a.queue > usqueue).ToArray();
                foreach(Reservation res in reservation)
                {
                    res.queue = res.queue - 1;
                }
                _context.Reservation.Remove(_context.Reservation.Where(a => a.user_id == id && a.book_id == book_id).First());
                _context.SaveChanges();
            }

            _context.User.Remove(user);
            user.is_valid=false;
            await _context.SaveChangesAsync();

            return Ok();
        }



        // nie zrobione
        //----------------------userdata

        [HttpGet("{id}"), Authorize]
        public IEnumerable<Rent> GetRent()
        {
            return _context.Rent;
        }
        [HttpGet("{id}"), Authorize]
        public IEnumerable<Reservation> GetReservation()
        {
            return _context.Reservation;
        }

        [HttpGet("{id}"), Authorize]
        public IEnumerable<Renth> GetRenth()
        {
            return _context.Renth;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                    res.expire_date = DateTime.Now.AddDays(8);
                    res.volume_id = reservation.volume_id;
                }
            }


            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }


        //zarzadzanie rezerwacjami
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservationnnn([FromRoute] int reservation_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter w = new BinaryWriter(fs);

            w.Write(_context.Reservation.FirstOrDefault().book_id);
            w.Close();
            if (_context.Reservation.Find(reservation_id).queue>0)
            {

                return BadRequest(new { alert = "Nie ma takiej rezerwacji" });
            }


            //     Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservation_id).FirstOrDefault();
            //    Reservation[] resToChange = _context.Reservation.Where(a => a.book_id == reservation.book_id && a.queue > reservation.queue).ToArray();
            //    foreach (Reservation res in resToChange)
            //    {
            //        res.queue = res.queue - 1;
            //        if (res.queue == 0)
            //        {
            //           res.is_active = true;
            //           res.expire_date = DateTime.Now.AddDays(8);
            //            res.volume_id = reservation.volume_id;
            //        }
            //     }
            //     _context.Reservation.Remove(reservation);
            //   await _context.SaveChangesAsync();
            return Ok("Rezerwacja została usunięta");
        }

        //    return this.http.delete(this.accessPointUrl + "/CancelReservation/" + id, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
        //}


        //-----------------------

        //------------NIEPOTRZEBNE

        //----------------------------
        // PUT: api/User/5
        [HttpPut]
        public async Task<IActionResult> UpdateUser( [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //new {alert= 
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

       
        // DELETE: api/User/5
      

        private bool UserExists(Int32 id)
        {
            return _context.User.Any(e => e.user_id.ToString() == id.ToString());
        }
    }
}