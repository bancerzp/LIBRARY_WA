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

        public List<String> GetLanguage()
        {
            return _context.Book.Select(a => a.language).Distinct().ToList();
        }

        [HttpGet("{isbn}")]
        public IEnumerable<Book> IfISBNExists([FromRoute] String isbn)
        {
            return _context.Book.Where(a => (a.isbn == isbn) ); //&& (a.is_available == true)
        }

        // GET: api/Books/5
        [HttpPost,Authorize]//"{book_id}/{ISBN}/{title}/{author_fullname}/{year}/{language}/{type}")]
        public IEnumerable<Book> SearchBook([FromBody] String[] search)
        {
            //  search.
            //FileStream fs = new FileStream("textt.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //BinaryWriter w = new BinaryWriter(fs);
            String[] name = { "book_id", "title", "ISBN", "author_fullname", "year", "language", "type" };
            String sql = "Select * from Book where is_available=true ";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    sql += "and " + name[i] + "='" + search[i] + "'";
                }
             
            }

            //w.Write(sql);
            //w.Close();

            return _context.Book.FromSql(sql);
        }


        [HttpPost]
        //  [Authorize(Roles = "l")]
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

        [HttpPost]
        public async Task<IActionResult> AddVolume([FromRoute] String id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = Convert.ToInt32(id);
            _context.Volume.Add(volume);
            _context.SaveChanges();
            return CreatedAtAction("AddVolume", new { id = volume.volume_id }, volume);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        //   [Authorize(Roles = "l")]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
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

            book.is_available=false;
           
            await _context.SaveChangesAsync();

            return Ok(book);
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



   