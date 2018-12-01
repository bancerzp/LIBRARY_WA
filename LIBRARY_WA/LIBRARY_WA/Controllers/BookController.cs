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

namespace LIBRARY_WA.Data
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        // get data to combobox
        [HttpGet]
        public List<String> GetAuthor()
        {
            return _context.Book.Where(a=>a.is_available==true).Select(a => a.author_fullname).Distinct().ToList();
        }

        [HttpGet]
        public List<String> GetBookType()
        {
            return _context.Book.Where(a => a.is_available == true).Select(a => a.type).Distinct().ToList();
        }

        [HttpGet]
        public List<String> GetLanguage()
        {
            return _context.Book.Where(a => a.is_available == true).Select(a => a.language).Distinct().ToList();
        }

        [HttpGet("{isbn}")]
        public IEnumerable<Book> IfISBNExists([FromRoute] String isbn)
        {
            return _context.Book.Where(a => (a.isbn == isbn)); //&& (a.is_available == true)
        }



        //BOOK function

        [HttpPost, Authorize(Roles = "l")]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Book.Where(a => a.isbn == book.isbn).Count() > 0)
            {
                return BadRequest("Książka o danym ISBN już istnieje w bazie danych.");
            }

            //  book.is_available = true;
            _context.Book.Add(book);
            await _context.SaveChangesAsync();
            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = book.book_id;
            _context.Volume.Add(volume);
            _context.SaveChanges();

            return CreatedAtAction("AddBook", new { id = book.book_id }, book);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVolumeByBookId([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var volume = _context.Volume.Where(a=>a.book_id==id);

            if (volume == null)
            {
                return NotFound();
            }

            return Ok(volume);
        }





        [HttpPost]
        public async Task<IActionResult> SearchBook([FromBody] String[] search)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //  search.
            FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter w = new BinaryWriter(fs);
            String[] name = { "book_id", "ISBN", "title", "author_fullname", "year", "language", "type" };
            String sql = "Select * from Book where is_available=true ";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    if (name[i] == "title")
                    {
                        String[] words = search[i].ToLower().Split(" ");
                        for (int j = 0; j < words.Length; j++)
                        {
                            sql += " and title like('%" + words[j] + "%') ";
                        }
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i] + "'";
                    }
                }

            }
            w.Write(sql);
            w.Close();
            var book = _context.Book.FromSql(sql);

            return Ok(book);
          

        }


        [HttpDelete("{id}"), Authorize(Roles = "l")]
        public async Task<IActionResult> RemoveBook([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { alert = "Nie znaleziono książki o danym id." });
            }

            if (_context.Rent.Where(a => a.book_id == id).Count() > 0)
            {
                return NotFound(new { alert = "Dana książka jest wypożyczona. Nie można jej usunąć" });
            }


            _context.Reservation.FromSql("DELETE from Reservation where book_id='" + id + "'");

            foreach (Volume volume in _context.Volume.Where(a => a.book_id == id))
            {
                volume.is_free = false;
            }
            book.is_available = false;
            //usuń wszystkie rezerwacje

            await _context.SaveChangesAsync();
            return Ok(book);
        }



        //Volume function
        [HttpPost("{id}")]//, Authorize(Roles = "l")]
        public async Task<IActionResult> AddVolume([FromRoute] int id)
        {

            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = id;
            _context.Volume.Add(volume);
            await _context.SaveChangesAsync();
            return CreatedAtAction("AddVolume", new { id = volume.volume_id }, volume);
            // return Ok(2);
        }

        [HttpGet]
        public async Task<IActionResult> GetVolume()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var volume = _context.Volume;

            if (volume == null)
            {
                return NotFound();
            }

            return Ok(volume);
        }

        //-----------------------


        [HttpDelete("{id}"), Authorize(Roles = "l")]
        public async Task<IActionResult> RemoveVolume([FromRoute] int volume_id)
        {
            //jeśli ma wypożyczone książki to komunikat, że nie można usunąć użytkownika bo ma nie wszystkie książki oddane, 
            //a jesli usunięty to zmienia isValid na false
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var volume = _context.Volume.Where(a=>a.volume_id==volume_id).First();
            if (volume == null)
            {
                return NotFound();
            }

            if (_context.Rent.Where(a => a.volume_id == volume_id).Count() > 0)
            {
                return NotFound(new { alert = "Dany egzemplarz jest wypożyczony. Nie można go usunąć" });
            }

            _context.Volume.Remove(volume);
            await _context.SaveChangesAsync();

            return Ok(volume);
        }


        [HttpPut, Authorize(Roles = "l,r")]
        public async Task<IActionResult> ReserveBook([FromBody] int[] data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Book.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return NotFound("Nie znaleziono książki o podanym id");
            }

            if (_context.User.Where(a => a.user_id == (data[1])).Count() == 0)
            {
                return NotFound("Nie znaleziono użytkownika o podanym id");
            }

            if (_context.Volume.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return NotFound("Książka nie ma żadnych egzemplarzy!");
            }

            Book book = _context.Book.Where(a => a.book_id == (data[0])).FirstOrDefault();
            int volume_id = _context.Volume.Where(a => a.book_id == data[0]).FirstOrDefault().volume_id;
            DateTime start_date = DateTime.Now;
            DateTime expire_date;
            int queue;
            Boolean is_active = true;
            if (_context.Volume.Where(a => a.book_id == data[0] && a.is_free == true).Count() == 0)
            {
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
            //    FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //    BinaryWriter w = new BinaryWriter(fs);
            //   w.Write(data[1]+"   user    "+ book.title + "    title    " + book.isbn + "  isbn     " + book.book_id + "  book_id    " + volume_id + "  volume     " + start_date + "   start    " + expire_date + "  stop     " + queue + "  kolejka     " + is_active);
            //   w.Close();
            Reservation reservation = new Reservation(data[1], book.title, book.isbn, book.book_id, volume_id, start_date, expire_date, queue, is_active);
            await _context.Reservation.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }



        [HttpPut("{id}"), Authorize(Roles = "l")]
        public async Task<IActionResult> RentBook([FromRoute] int reservationId)
        {

            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }

            if (_context.Reservation.Where(a => a.reservation_id == reservationId).Count() == 0)
            {
                return Ok(new { error="Nie ma takiej rezerwacji" });
            }

            Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservationId).FirstOrDefault();
            if (_context.Volume.Where(a => a.is_free == true).Count() == 0)
            {
                return Ok(new { error = "Nie ma takiego egzemplarza" });
            }
            int volume_id = _context.Volume.Where(a => a.is_free == true).FirstOrDefault().volume_id;
            //zmień is free na false
            _context.Volume.Where(a => a.is_free == true).FirstOrDefault().is_free = false;
            Rent rent = new Rent(reservation.user_id, reservation.book_id, reservation.title, reservation.isbn, volume_id, DateTime.Now, DateTime.Now.AddMonths(1));
            await _context.Rent.AddAsync(rent);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}"), Authorize(Roles = "l")]
        public async Task<IActionResult> ReturnBook([FromRoute] int rent_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Rent.Where(a => a.rent_id == rent_id).Count() == 0)
            {
                return BadRequest(new { error = "Nie ma takiego wypożyczenia" });
            }

            Rent rent = _context.Rent.Where(a => a.rent_id == rent_id).FirstOrDefault();
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

            await _context.SaveChangesAsync();
            return Ok("Książka została poprawnie zwrócona");

        }

     


        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.book_id == id);
        }

        //  [Authorize(Roles = "l")]
        //wydanie książki użytkownikowi
        //jeśli książka niedostępna to zarezerwuj, jeśli dostępna to wypożycz
        /*  public  RentBook()
          {

          }


          //rezerwacja książki
          [Authorize(Roles = "l,u")]
          public ReserveBook()
          {

          }
        [Authorize(Roles = "l,u")]
        //ReturnBook(){
        }
        */

        
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



