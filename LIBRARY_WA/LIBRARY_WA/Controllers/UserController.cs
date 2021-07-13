using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Library.Services;
using Library.Models.DTO;

namespace Library.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly UserService _userService;

        public UserController(UserService userService, IConfiguration configuration)
        {
            Configuration = configuration;
            _userService = userService;
        }

        //----Data verifying
        [HttpGet("{email}")]
        public IActionResult IfEmailExists([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(_userService.IfEmailExists(email));

        }

        [HttpGet("{login}")]
        public IActionResult IfLoginExists([FromRoute] string login)
        {
            return Ok(_userService.IfLoginExists(login));
        }

        // GET: api/User
        [HttpPost]
        public IActionResult IsLogged([FromBody] UserDTO userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { alert = "Nieprawidłowe dane logowania" });
            }

            var user = _userService.GetLoggedInfo(userData.Login, userData.Password);

            if (user is null)
            {
                return Unauthorized();
            }

            if (userData.IsValid)
            {
                return BadRequest(new { alert = "Użytkownik jest zablokowany. Prosimy o kontakt z biblioteką." });
            }

            return Ok(user);
        }

        [HttpPut, Authorize(Roles = "l")]
        public ActionResult ChangeUserStatus(StatusDTO status)
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

            if (user is null)
            {
                return NotFound(new { alert = "Użytkownik o danym id nie istnieje!" });
            }

            return Ok(user);
        }

        [HttpPost]
      //  [HttpPost, Authorize(Roles = "l")]
        public ActionResult<UserDTO> SearchUser([FromBody] String[] search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_userService.SearchUser(search));
        }

        [HttpPost, Authorize(Roles = "l")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string message = _userService.AddUserCheckData(user);
            if (message != "")
            {
                return BadRequest(new { alert = message });
            }

            var r = _userService.AddUser(user);
            return CreatedAtAction("AddUser", new { id = r.UserId }, r);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string message = _userService.RemoveUserCheckData(id);
            if (message != "")
            {
                return NotFound(new { alert = message });
            }

            _userService.RemoveUser(id);
            return Ok();
        }

        [HttpGet("{id}"), Authorize]
        public ActionResult<IEnumerable<RentDTO>> GetRent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.GetRents(id));
        }

        [HttpGet("{id}"), Authorize]
        public ActionResult<IEnumerable<ReservationDTO>> GetReservation([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.GetReservations(id));
        }

        [HttpGet("{id}"), Authorize]
        public ActionResult<IEnumerable<RenthDTO>> GetRenth([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.GetRentsHistory(id));
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
        public ActionResult UpdateUser([FromBody] UserDTO user)
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
        public ActionResult ResetPassword([FromBody] UserDTO user)
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