using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RentingController : ControllerBase
    {
        private RentingService _rentingService;

        public RentingController(RentingService rentingService)
        {
            _rentingService = rentingService;
        }

        [HttpPost, Authorize(Roles = "l")] //, , 
        public async Task<IActionResult> ReturnBook([FromBody] int[] rentId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_rentingService.ReturnBookCheckCondition(rentId))
            {
                return NotFound(new { alert = "Nie ma takiego wypożyczenia" });
            }

            await _rentingService.ReturnBook(rentId);
            return Ok(new { message = "Książka została poprawnie zwrócona" });
        }

        [HttpPut, Authorize(Roles = "l")] //, 
        public ActionResult RentBook([FromBody] int[] reservationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_rentingService.IsUserBlocked(reservationId[0]))
            {
                return BadRequest(new { alert = "Nie można wypożyczyć książki. Użytkownik jest zablokowany!" });
            }

            string answear = _rentingService.RentBookCheckCondition(reservationId);
            if (answear != "")
            {
                return NotFound(new { alert = answear });
            }
            _rentingService.RentBook(reservationId);
            return Ok();
        }
    }
}