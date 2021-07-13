using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Library.Services;

namespace Library.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private BookService _bookService;
        private RentingService _rentingService;

        public BookController(BookService bookService, RentingService rentingService)
        {
            _bookService = bookService;
            _rentingService = rentingService;
        }

        [HttpGet("{isbn}")]
        public ActionResult<bool> IfISBNExists([FromRoute] string isbn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return _bookService.IfISBNExists(isbn);
        }

        //BOOK function

        [HttpPost, Authorize(Roles = "l")]
        public ActionResult<BookDTO> AddBook([FromBody] BookDTO book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_bookService.IfISBNExists(book.Isbn))
            {
                return BadRequest(new { alert = "Książka o danym ISBN już istnieje w bazie danych." });
            }
            _bookService.AddBook(book);

            return CreatedAtAction("AddBook", new { id = book.BookId }, book);
        }

        [HttpGet("{id}")]
        public ActionResult<BookDTO> GetBookById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BookDTO book = _bookService.GetBookById(id);

            if (book == null)
            {
                return NotFound(new { alert = "Książka o danym id nie istnieje!" });
            }

            return Ok(book);
        }

        [HttpGet("{id}")]
        public ActionResult<List<VolumeDTO>> GetVolumeByBookId([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<VolumeDTO> volume = _bookService.GetVolumeByBookId(id);

            if (volume == null)
            {
                return NotFound();
            }

            return Ok(volume);
        }

        [HttpPost]
        public ActionResult<BookDTO> SearchBook([FromBody] String[] search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_bookService.SearchBook(search));
        }

        [HttpDelete("{id}"), Authorize(Roles = "l")]
        public ActionResult RemoveBook([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_bookService.GetBookById(id) == null)
            {
                return NotFound(new { alert = "Nie znaleziono książki o danym id." });
            }

            if (!_rentingService.IsRentExist(id))
            {
                return NotFound(new { alert = "Dana książka jest wypożyczona. Nie można jej usunąć" });
            }

            _bookService.RemoveBook(id);
            return Ok();
        }

        //Volume function
        [HttpPost, Authorize(Roles = "l")]//, ]
        public ActionResult<VolumeDTO> AddVolume([FromBody] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VolumeDTO volume = _bookService.AddVolume(id);
            return CreatedAtAction("AddVolume", new { id = volume.VolumeId }, volume);
        }
        /*
                [HttpGet]
                public async Task<IActionResult> GetVolume()
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var volume = _context.Volume;

                    return Ok(volume);
                }
                */
        //-----------------------


        [HttpDelete("{id}"), Authorize(Roles = "l")]//
        public ActionResult<VolumeDTO> RemoveVolume([FromRoute] int id)
        {
            //jeśli ma wypożyczone książki to komunikat, że nie można usunąć użytkownika bo ma nie wszystkie książki oddane, 
            //a jesli usunięty to zmienia isValid na false

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string answear = _bookService.RemoveVolumeCheckCondition(id);
            if (answear != "")
            {
                return NotFound(new { alert = answear });
            }

            _bookService.RemoveVolume(id);
            return Ok();
        }

        [HttpGet("{userId}"), Authorize(Roles = "l,r")]
        public async Task<IActionResult> GetSuggestion([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _bookService.GetSuggestionCheckCondition(userId);
            var suggestion = _bookService.GetSuggestion(userId);

            return Ok(suggestion);
        }

        private bool BookExists(int bookId)
        {
            return _bookService.BookExists(bookId);
        }

        [HttpPut, Authorize(Roles = "l")]
        public async Task<IActionResult> UpdateBook([FromBody] BookDTO book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_bookService.BookExists(book.BookId))
            {
                return NotFound("Nie znaleziono książki!");
            }

            try
            {
                _bookService.UpdateBook(book);
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return Ok();
        }
    }
}