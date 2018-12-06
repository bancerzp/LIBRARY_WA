using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.IO;
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
        public ActionResult<Boolean> IfISBNExists([FromRoute] String isbn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return _bookService.IfISBNExists(isbn);
        }
        
        //BOOK function

        [HttpPost, Authorize(Roles = "l")]
        public ActionResult<Book_DTO>  AddBook([FromBody] Book_DTO book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_bookService.IfISBNExists(book.isbn))
            {
                return BadRequest(new {alert= "Książka o danym ISBN już istnieje w bazie danych." });
            }
            _bookService.AddBook(book);
            
            return CreatedAtAction("AddBook", new { id = book.book_id }, book);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = GetBookById(id);

            if (book == null)
            {
                return NotFound(new { alert = "Książka o danym id nie istnieje!" });
            }

            return Ok(book);
        }

        [HttpGet("{id}")]
        public ActionResult<Volume> GetVolumeByBookId([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Volume_DTO volume = _bookService.GetVolumeByBookId(id);

            if (volume == null)
            {
                return NotFound();
            }

            return Ok(volume);
        }


        [HttpPost]
        public ActionResult<Book> SearchBook([FromBody] String[] search)
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

            Volume_DTO volume= _bookService.AddVolume(id);
            return CreatedAtAction("AddVolume", new { id = volume.volume_id }, volume);
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
        public async Task<IActionResult> RemoveVolume([FromRoute] int id)
        {
            //jeśli ma wypożyczone książki to komunikat, że nie można usunąć użytkownika bo ma nie wszystkie książki oddane, 
            //a jesli usunięty to zmienia isValid na false
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (_context.Volume.Where(a => a.volume_id == id).Count() == 0)
            {
                return NotFound(new { alert = "Egzemplarz o danym id nie istnieje!" });
            }

            var volume = _context.Volume.Where(a => a.volume_id == id).FirstOrDefault();

            if (_context.Rent.Where(a => a.volume_id == id).Count() > 0)
            {
                return NotFound(new { alert = "Dany egzemplarz jest wypożyczony. Nie można go usunąć!" });
            }

            if (_context.Reservation.Where(a => a.volume_id == id && a.is_active==true).Count() > 0)
            {
                Reservation reservation = _context.Reservation.Where(a => a.volume_id == id && a.is_active == true).FirstOrDefault();

                foreach (Reservation reserv in _context.Reservation.Where(a => a.book_id == volume.book_id && a.is_active == false)) {
                    reserv.queue = reserv.queue + 1;
                }
                if(_context.Volume.Where(a => a.book_id == volume.book_id).Count() > 1)
                {
                    var n = _context.Volume.Where(a => a.book_id == volume.book_id && a.volume_id!=id).FirstOrDefault();
                    Reservation r = new Reservation(reservation.user_id, reservation.title, reservation.isbn, reservation.book_id, n.volume_id, reservation.start_date, reservation.expire_date, 1, false);
                    r.reservation_id = reservation.reservation_id;
                    _context.Reservation.Remove(reservation);
                    _context.SaveChanges();
                    _context.Reservation.Add(r);
                }
                else
                {
                    _context.Reservation.Remove(reservation);
                }
               
                _context.SaveChanges();
            }
            
            _context.Volume.Remove(volume);
            await _context.SaveChangesAsync();

            return Ok(volume);
        }


        [HttpPut, Authorize(Roles = "l,r")]
        public async Task<IActionResult> ReserveBook([FromBody] int[] data)
        {
            //FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
           // BinaryWriter w = new BinaryWriter(fs);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            if (_context.Book.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return NotFound(new { alert = "Nie znaleziono książki o podanym id" });
            }
          
            if (_context.User.Where(a => a.user_id == (data[1])).Count() == 0)
            {
                return NotFound(new { alert = "Nie znaleziono użytkownika o podanym id" });
            }

            if (_context.Reservation.Where(a => a.book_id == data[0] && a.user_id==data[1]).Count()> 0)
            {
                return BadRequest(new { alert = "Użytkownik ma już zarezerwowaną tę książkę!" });
            }

            if (_context.Volume.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return NotFound(new { alert = "Książka nie ma żadnych egzemplarzy!" });
            }
          
            Book book = _context.Book.Where(a => a.book_id == (data[0])).FirstOrDefault();
           
            int volume_id = _context.Volume.Where(a => a.book_id == data[0]).FirstOrDefault().volume_id;
          
            DateTime start_date = DateTime.Now;
            DateTime expire_date;
            int queue;
            Boolean is_active = true;
          
            
            if (_context.Volume.Where(a => a.book_id == data[0] && a.is_free == true).Count() == 0)
            {
                if (_context.Reservation.Where(a => a.book_id == data[0]).OrderByDescending(a => a.queue).FirstOrDefault() == null)
                    queue = 1;
                else
                 queue = _context.Reservation.Where(a => a.book_id == data[0]).OrderByDescending(a => a.queue).FirstOrDefault().queue + 1;
                is_active = false;
                expire_date = DateTime.Now;
            }
            else
            {
                volume_id = _context.Volume.Where(a => a.book_id == Convert.ToInt32(data[0]) && a.is_free == true).FirstOrDefault().volume_id;
                expire_date = DateTime.Now.AddDays(14);
                _context.Volume.Where(a => a.volume_id == volume_id).FirstOrDefault().is_free = false;
                queue = 0;
              
            }
          
         //   FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
           //     BinaryWriter w = new BinaryWriter(fs);
            //   w.Write(data[1]+"   user    "+ book.title + "    title    " + book.isbn + "  isbn     " + book.book_id + "  book_id    " + volume_id + "  volume     " + start_date + "   start    " + expire_date + "  stop     " + queue + "  kolejka     " + is_active);
              
            Reservation reservation = new Reservation(data[1], book.title, book.isbn, book.book_id, volume_id, start_date, expire_date, queue, is_active);
            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();
           
            return Ok(reservation);
        }



        [HttpPut, Authorize(Roles = "l")] //, 
        public async Task<IActionResult> RentBook([FromBody] int[] reservation_id)
        {
           
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            if (_context.Reservation.Where(a => a.reservation_id == reservation_id[0]).Count() == 0)
            {
                return BadRequest(new { alert="Nie ma takiej rezerwacji" });
            }

            Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservation_id[0]).FirstOrDefault();
            if (_context.Volume.Where(a => a.is_free == true).Count() == 0)
            {
                return BadRequest(new { alert = "Nie ma takiego egzemplarza" });
            }
            int volume_id = _context.Volume.Where(a => a.is_free == true).FirstOrDefault().volume_id;
            //zmień is free na false
            _context.Volume.Where(a => a.is_free == true).FirstOrDefault().is_free = false;
            Rent rent = new Rent(reservation.user_id, reservation.book_id, reservation.title, reservation.isbn, volume_id, DateTime.Now, DateTime.Now.AddMonths(1));
            await _context.Rent.AddAsync(rent);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost, Authorize(Roles = "l")] //, , 
        public async Task<IActionResult> ReturnBook([FromBody] int[] rent_id)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Rent.Where(a => a.rent_id == rent_id[0]).Count() == 0)
            {
                return BadRequest(new { alert = "Nie ma takiego wypożyczenia" });
            }

            Rent rent = _context.Rent.Where(a => a.rent_id == rent_id[0]).FirstOrDefault();
            Volume volume = _context.Volume.Where(a => a.volume_id == rent.volume_id).FirstOrDefault();
            Reservation[] reservations = _context.Reservation.Where(a => a.book_id == rent.book_id).ToArray();
            Renth renth = new Renth(rent.user_id, rent.title, rent.isbn, rent.book_id, rent.volume_id, rent.start_date, DateTime.Now);
            //wstaw do historii rezerwacji
            await _context.Renth.AddAsync(renth);
            await _context.SaveChangesAsync();

            //z rezerwacji pozmieniać numerki, który w kolejce-->czy gotowa do odebrania na tak i zmienić queue
            // this.expire_date = expire_date; data dziiejsza plus tydzien
            //this.queue = queue;  -1
            //this.is_active = is_active;
            foreach (Reservation reservation in reservations)
            {
                reservation.queue = reservation.queue - 1;
                if (reservation.queue == 0)
                {
                    reservation.is_active = true;
                    reservation.expire_date = DateTime.Now.AddDays(8);
                    reservation.volume_id = volume.volume_id;
                }
            }

            //usuń z rent
            _context.Rent.Remove(rent);
            _context.SaveChanges();
            return Ok(new { message = "Książka została poprawnie zwrócona" });
        }

        [HttpGet("{user_id}"), Authorize(Roles = "l,r")]
        public async Task<IActionResult> GetSuggestion([FromRoute] int user_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.User.Where(a => a.user_id == user_id).Count() == 0)
            {
                return BadRequest(new { alert = "Nie ma takiego użytkownika" });
            }
            var sql = "CALL Get_suggestion(" + user_id + ")";
            _context.Database.ExecuteSqlCommand(sql);
            var suggestion = _context.Suggestion;
            return Ok(suggestion);

//            select top 5 from(
//            select * from book where AUTHOR_FULLNAME in (select distinct AUTHOR_FULLNAME from renth re, book bo where user_id = 2 and re.BOOK_ID = bo.book_id)

//            union

//            select* from book where type in (select distinct type from renth re,book bo where user_id = 2 and re.BOOK_ID = bo.book_id) 
//            select* from book where type in (select distinct type from renth re,book bo where user_id = 2 and re.BOOK_ID = bo.book_id) 
//			union

//            select* from book where language in (select distinct language from renth re,book bo where user_id = 2 and re.BOOK_ID = bo.book_id)) a
//   where a.book_id not in(select r.book_id
//                           from renth r, book b
//                            where r.user_id = 2
//                        and r.title = b.title
//                        and b.AUTHOR_FULLNAME = (select AUTHOR_FULLNAME
//                                                  from book bo
//                                                where r.book_id = bo.book_id) )
//and is_active = true
//group by title
        }


        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.book_id == id);
        }

        [HttpPut, Authorize(Roles = "l")]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Book.Where(a => a.book_id == book.book_id).Count() == 0)
            {
                return NotFound("Nie znaleziono książki!");
            }
         

            //new {alert= 
            _context.Entry(book).State = EntityState.Modified;
            _context.Book.Update(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return Ok();
        }





        // PUT: api/Book/5
        [HttpPut]
        public async Task<IActionResult> EditBook( [FromBody] Book book)
        {
           if (!ModelState.IsValid)
           {
               return BadRequest(ModelState);
           }

           _context.Entry(book).State = EntityState.Modified;

           try
           {
               await _context.SaveChangesAsync();
           }
           catch (DbUpdateConcurrencyException)
           {
               if (_context.Book.Where(a=>a.book_id==book.book_id).Count()==0)
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
        }
    }



