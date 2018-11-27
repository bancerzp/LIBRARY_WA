using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace LIBRARY_WA.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        // private readonly UserContext _context;
        private readonly LibraryContext _context;

        public UserController(LibraryContext context, IConfiguration configuration)
        {
            Configuration = configuration;

            this._context = context;//.UserContext;
         //   this.context = context;
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
                return BadRequest("Invalid client request");
            }
            User user = _context.User.Where(u => u.login == userData.login && u.password == userData.password).FirstOrDefault();
            if (user != null)

            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
        {
                 new Claim(ClaimTypes.Name, user.fullname),
                 new Claim(ClaimTypes.Role, user.user_Type),
        };

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5000",
                    audience: "http://localhost:5000",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString, id = user.user_Id, user_type = user.user_Type, fullname = user.fullname });
            }
            else
            {
                return Unauthorized();
            }

        }


        [HttpGet("{id}"), Authorize]
        public IEnumerable<User> GetUserById([FromRoute] Int32 id)
        {
            //rom p in context.Professors
            // select p.Name).ToList()

            return _context.User.Where(a => a.user_Id == id);
        }



        [HttpPost, Authorize]
        public IEnumerable<User> SearchUser([FromBody] String[] search)
        {
            String[] name = { "user_id", "login", "fullname", "date_of_birth", "phone_number", "email" };
            String sql = "Select * from User where 1=1 ";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    if (name[i] == "fullname")
                    {
                        sql += "and " + name[i] + "like('%" + search[i] + "%') ";
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i] + "' ";
                    }
                }
            }
            return _context.User.FromSql(sql);


        }



        // POST: api/User
        [HttpPost,Authorize(Roles ="l")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.user_Id }, user);
        }


      

        //----------------------userdata
      
        [HttpGet("{id}")]
        public IEnumerable<Rent> GetRent()
        {
            return _context.Rent;
        }
        [HttpGet("{id}")]
        public IEnumerable<Reservation> GetReservation()
        {
            return _context.Reservation;
        }

        [HttpGet("{id}")]
        public IEnumerable<Renth> GetRenth()
        {
            return _context.Renth;
        }


        // GET: api/User/5
        [HttpGet("{id}")]
        public  IEnumerable<User> GetUser([FromRoute] string id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var user = await _context.User.FindAsync(id);

            //if (user == null)
            //{
            //    return NotFound();
            //}
            return _context.User;
        }










        //----------------------------
        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] Int32 id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id.ToString() == user.user_Id.ToString())
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       
        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {

            //jeśli ma wypożyczone książki to komunikat, że nie można usunąć użytkownika bo ma niewyszystkie książki oddane, 
            //a jesli usunięty to zmienia isValid na false
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(Int32 id)
        {
            return _context.User.Any(e => e.user_Id.ToString() == id.ToString());
        }
    }
}