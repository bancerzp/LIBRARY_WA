using Library.Services;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPut, Authorize(Roles = "l,r")]
        public ActionResult<ReservationDTO> ReserveBook([FromBody] int[] data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string answear = _reservationService.ReserveBookCheckCondition(data);
            if (answear != "")
            {
                if (answear == "Użytkownik ma już zarezerwowaną tę książkę!")
                {
                    return BadRequest(new { alert = answear });
                }
                return NotFound(new { alert = answear });
            }


            return Ok(_reservationService.ReserveBook(data));
        }
    }
}