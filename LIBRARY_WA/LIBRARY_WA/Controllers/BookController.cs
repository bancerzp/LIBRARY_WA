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

namespace LIBRARY_WA.Data
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookContext _context;

        public BookController(BookContext context)
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

        [HttpGet("{ISBN}")]
        public IEnumerable<Book> IfISBNExists([FromRoute] String ISBN)
        {
            return _context.Book.Where(a => a.ISBN == ISBN);
        }

        // GET: api/Books/5
        [HttpGet]//"{book_id}/{ISBN}/{title}/{author_fullname}/{year}/{language}/{type}")]
        public  IEnumerable<Book> SearchBook([FromHeader] String[] search )
        {
            String[] name= { "book_id","title", "ISBN", "author_fullname", "year", "language", "type" };
            String sql= "Select * from Book where 1=1 ";
            for(int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    sql += "and " + name[i] + "='" + search[i] + "'";
                }
            }
            return _context.Book.FromSql(sql);
        }

        // PUT: api/Book/5
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
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          //  book.is_available = true;
            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("AddBook", new { id = book.book_id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
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

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(book);
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.book_id == id);
        }
    }
}