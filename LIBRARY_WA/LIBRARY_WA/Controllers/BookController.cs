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

        // GET: api/Book
        [HttpGet]
        public List<String> GetAuthor()
        {
            return _context.Book.Select(a => a.author_fullname).Distinct().ToList();
        }

        [HttpGet]
        public List<String> GetBookType()
        {
            return _context.Book.Select(a => a.type).Distinct().ToList();
        }

        [HttpGet]
        public List<String> GetLanguage()
        {
            return _context.Book.Select(a => a.language).Distinct().ToList();
        }

        [HttpGet("{isbn}")]
        public IEnumerable<Book> IfISBNExists([FromRoute] String isbn)
        {
            return _context.Book.Where(a => (a.isbn == isbn) ); //&& (a.is_available == true)
        }

        
        [HttpPost]
        public async Task<IActionResult> SearchBook([FromBody] String[] search)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //  search.
            //FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //BinaryWriter w = new BinaryWriter(fs);
            String[] name = { "book_id", "ISBN", "title", "author_fullname", "year", "language", "type" };
            String sql = "Select * from Book where is_available=true ";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    sql += "and " + name[i] + "='" + search[i] + "'";
                }
             
            }
            var book = _context.Book.FromSql(sql);

            return Ok(book);
            //w.Write(sql);
            //w.Close();
            
        }


        [HttpPost,Authorize(Roles = "l")]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

        [HttpPost, Authorize(Roles = "l")]
        public async Task<IActionResult> AddVolume([FromRoute] String id)
        {
          
            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = Convert.ToInt32(id);
            _context.Volume.Add(volume);
            await _context.SaveChangesAsync();
            return CreatedAtAction("AddVolume", new { id = volume.volume_id }, volume);
        }

        
        [HttpDelete("{id}"),Authorize(Roles = "l")]
        public async Task<IActionResult> RemoveBook([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound(new {alert="Nie znaleziono książki o danym id." });
            }

            if (_context.Rent.Where(a=>a.book_id==id).Count()>0)
            {
                return NotFound(new { alert = "Dana książka jest wypożyczona. Nie można jej usunąć" });
            }


            _context.Reservation.FromSql("DELETE from Reservation where book_id='"+id+"'");

            foreach (Volume volume in _context.Volume.Where(a => a.book_id == id)) {
                volume.is_free = false;
            }
            book.is_available=false;
           //usuń wszystkie rezerwacje
       
            await _context.SaveChangesAsync();
            return Ok(book);
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

            if (_context.Rent.Where(a => a.volume_id == volume_id).Count() > 0)
            {
                return NotFound(new { alert = "Dany egzemplarz jest wypożyczony. Nie można go usunąć" });
            }

            var volume = await _context.Volume.FindAsync(volume_id);
            if (volume == null)
            {
                return NotFound();
            }

            _context.Volume.Remove(volume);
            await _context.SaveChangesAsync();

            return Ok(volume);
        }

        [HttpPut, Authorize(Roles = "l,r")]
        public async Task<IActionResult> ReserveBook([FromBody] String[] data)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Book.Where(a => a.book_id.Equals(data[0])).Count() == 0)
            {
                return NotFound("Nie znaleziono książki o podanym id");
            }

            if (_context.User.Where(a => a.user_Id.Equals(data[1])).Count() == 0)
            {
                return NotFound("Nie znaleziono użytkownika o podanym id");
            }

            Book book = _context.Book.Where(a=>a.book_id.Equals(data[0])).FirstOrDefault();
            Int32 volume_id = _context.Volume.Where(a => a.book_id == Convert.ToInt32(data[0]) && a.is_free == true).FirstOrDefault().volume_id;
            DateTime start_date =DateTime.Now;
            DateTime expire_date;
            Int32 queue;
            Boolean is_active = true;
            if (_context.Volume.Where(a => a.book_id == Convert.ToInt32(data[0]) && a.is_free == true).Count() == 0)
            {
                queue = Convert.ToInt32(_context.Reservation.FromSql("Select queue from reservation where book_id='" + data[0] + "' order by queue desc;")) + 1;
                is_active = false;
                expire_date = DateTime.Now;
            }
            else
            {
                expire_date = DateTime.Now.AddDays(14);
                _context.Volume.Where(a => a.book_id == Convert.ToInt32(data[0]) && a.is_free == true).FirstOrDefault().is_free=false;
                queue = 0;
            }
            Reservation reservation = new Reservation( book.title, book.isbn, book.book_id, volume_id, start_date, expire_date, queue, is_active);
            await _context.Reservation.AddAsync(reservation);
            await _context.SaveChangesAsync();
           
            return Ok(reservation);
        }

        [HttpPut("{id}"), Authorize(Roles = "l")]
        public async Task<IActionResult> RentBook([FromRoute] int reservationId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(_context.Reservation.Where(a => a.reservation_id == reservationId).Count() == 0)
            {
                return BadRequest("Nie ma takiej rezerwacji");
            }
            Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservationId).FirstOrDefault();
            if (_context.Volume.Where(a => a.is_free == true).Count() == 0)
            {
                return BadRequest(ModelState);
            }
            Int32 volume_id = _context.Volume.Where(a=>a.is_free==true).FirstOrDefault().volume_id;
            Rent rent = new Rent(reservation.user_id, reservation.book_id, reservation.title, reservation.isbn, volume_id, DateTime.Now, DateTime.Now.AddMonths(1));
            await _context.Rent.AddAsync(rent);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> ReturnBook([FromRoute] int rent_id)
        //{
        //    return this.http.post(this.accessPointUrl + "/ReturnBook/" + id, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> CancelReservation([FromRoute] int reservation_id)
        //{
        //    return this.http.delete(this.accessPointUrl + "/CancelReservation/" + id, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
        //}
        ////-----------------------


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

        /*
        *   // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook([FromRoute] int id, [FromBody] Book book)
        {
           if (!ModelState.IsValid)
           {
               return BadRequest(ModelState);
           }

           if (id != book.book_id)
           {
               return BadRequest();
           }

           _context.Entry(book).State = EntityState.Modified;

           try
           {
               await _context.SaveChangesAsync();
           }
           catch (DbUpdateConcurrencyException)
           {
               if (!BookExists(id))
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

        // POST: api/Books
        }

        */
    }
}



   