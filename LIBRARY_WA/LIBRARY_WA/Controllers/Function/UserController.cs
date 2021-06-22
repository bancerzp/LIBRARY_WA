using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using LIBRARY_WA.Controllers.Services;
using LIBRARY_WA.Models.DTO;

namespace LIBRARY_WA.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; }

       
        private readonly UserService _userService;

        public UserController(UserService _userService, IConfiguration configuration)
        {
            Configuration = configuration;
            this._userService = _userService;
        }


        //----Data verifying
        [HttpGet("{email}")]
        public IActionResult IfEmailExists([FromRoute] String email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
           
            return Ok(_userService.IfEmailExists(email));

        }

        [HttpGet("{login}")]
        public IActionResult IfLoginExists([FromRoute] String login)
        {
            String login2 = login.Replace("'", "");
            return Ok(_userService.IfLoginExists(login));
        }


        // GET: api/User
        [HttpPost]
        public IActionResult IsLogged([FromBody] User_DTO userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { alert = "Nieprawidłowe dane logowania" });
            }

            if(!_userService.IsBlocked(userData))
            {
                return BadRequest(new { alert = "Użytkownik jest zablokowany. Prosimy o kontakt z biblioteką." });
            }

            if (_userService.IsLoggedCheckData(userData)){
                var toReturn = _userService.IsLogged(userData);
                return Ok(new { Token = toReturn.Token, id = toReturn.id, user_type = toReturn.user_type, fullname = toReturn.fullname, expires= toReturn.expires });
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPut, Authorize(Roles = "l")]
        public ActionResult ChangeUserStatus(Status_DTO status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _userService.ChangeUserStatus(status);
            return Ok();
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound(new { alert = "Użytkownik o danym id nie istnieje!" });
            }

            return Ok(user);
        }



        [HttpPost, Authorize(Roles = "l")]
        public ActionResult<User_DTO> SearchUser([FromBody] String[] search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

         
            return Ok(_userService.SearchUser(search));
        }

      
        [HttpPost,Authorize(Roles ="l")]
        public async Task<IActionResult> AddUser([FromBody] User_DTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            String message = _userService.AddUserCheckData(user);
            if (message != "")
            {
                return BadRequest(new { alert= message });
            }

            var r = _userService.AddUser(user);
            return CreatedAtAction("AddUser", new { id = r.user_id }, r);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            String message = _userService.RemoveUserCheckData(id);
            if (message!="")
            {
                return NotFound(new { alert = message });
            }

            _userService.RemoveUser(id);
            return Ok();
        }


        [HttpGet("{id}"), Authorize]
        public ActionResult<IEnumerable<Rent_DTO>> GetRent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.GetRent(id));
        }

        [HttpGet("{id}"), Authorize]
        public ActionResult<IEnumerable<Reservation_DTO>> GetReservation([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.GetReservation(id));
        }

        [HttpGet("{id}"), Authorize]
        public ActionResult<IEnumerable<Renth_DTO>> GetRenth([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.GetRenth(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            if (!_userService.CancelReservationCheckData(id))
            {
                return NotFound(new { alert = "Nie istnieje rezerwacja o danym id!" });
            }

            _userService.CancelReservation(id);
            return Ok(id);
        }

       
        [HttpPut]
        public ActionResult UpdateUser( [FromBody] User_DTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         
            try
            {
                _userService.UpdateUser(user);
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return NoContent();
        }

        [HttpPut]
        public ActionResult ResetPassword([FromBody] User_DTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                _userService.UpdateUser(user);
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }
    }
}