﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using LIBRARY_WA.Controllers.Services;

namespace LIBRARY_WA.Data
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        // get data to combobox
        [HttpGet]
        public List<String> GetAuthor()
        {
            return _bookService.GetAuthor();
        }

        [HttpGet]
        public List<String> GetBookType()
        {
            return _bookService.GetBookType();
        }

        [HttpGet]
        public List<String> GetLanguage()
        {
            return _bookService.GetLanguage();
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
        public ActionResult<Book_DTO> AddBook([FromBody] Book_DTO book)
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
        public ActionResult<Book_DTO> GetBookById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Book_DTO book = _bookService.GetBookById(id);

            if (book == null)
            {
                return NotFound(new { alert = "Książka o danym id nie istnieje!" });
            }

            return Ok(book);
        }

        [HttpGet("{id}")]
        public ActionResult<List<Volume_DTO>> GetVolumeByBookId([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Volume_DTO> volume = _bookService.GetVolumeByBookId(id);

            if (volume == null)
            {
                return NotFound();
            }

            return Ok(volume);
        }


        [HttpPost]
        public ActionResult<Book_DTO> SearchBook([FromBody] String[] search)
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

            if (!_bookService.GetRentById(id))
            {
                return NotFound(new { alert = "Dana książka jest wypożyczona. Nie można jej usunąć" });
            }

            _bookService.RemoveBook(id);
            return Ok();
        }



        //Volume function
        [HttpPost, Authorize(Roles = "l")]//, ]
        public ActionResult<Volume_DTO> AddVolume([FromBody] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Volume_DTO volume = _bookService.AddVolume(id);
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
        public ActionResult<Volume_DTO> RemoveVolume([FromRoute] int id)
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


        [HttpPut, Authorize(Roles = "l,r")]
        public ActionResult<Reservation_DTO> ReserveBook([FromBody] int[] data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string answear = _bookService.ReserveBookCheckCondition(data);
            if (answear != "")
            {
                if (answear == "Użytkownik ma już zarezerwowaną tę książkę!")
                {
                    return BadRequest(new { alert = answear });
                }
                return NotFound(new { alert = answear });
            }


            return Ok(_bookService.ReserveBook(data));
        }



        [HttpPut, Authorize(Roles = "l")] //, 
        public ActionResult RentBook([FromBody] int[] reservationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_bookService.IsBlocked(reservationId[0]))
            {
                return BadRequest(new { alert = "Nie można wypożyczyć książki. Użytkownik jest zablokowany!" });
            }

            string answear = _bookService.RentBookCheckCondition(reservationId);
            if (answear != "")
            {
                return NotFound(new { alert = answear });
            }
            _bookService.RentBook(reservationId);
            return Ok();
        }

        [HttpPost, Authorize(Roles = "l")] //, , 
        public async Task<IActionResult> ReturnBook([FromBody] int[] rentId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_bookService.ReturnBookCheckCondition(rentId))
            {
                return NotFound(new { alert = "Nie ma takiego wypożyczenia" });
            }

            await _bookService.ReturnBook(rentId);
            return Ok(new { message = "Książka została poprawnie zwrócona" });
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


        private bool BookExists(int id)
        {
            return _bookService.BookExists(id);
        }

        [HttpPut, Authorize(Roles = "l")]
        public async Task<IActionResult> UpdateBook([FromBody] Book_DTO book)
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