using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.EntityFrameworkCore.Internal;
using LIBRARY_WA.Models.database;

namespace LIBRARY_WA.Controllers.Services
{
    public class BookService : ControllerBase
    {
        private readonly LibraryContext _context;

        public BookService(LibraryContext context)
        {
            _context = context;
        }

        public bool IfISBNExists(string isbn)
        {
            return _context.Book.Where(a => (a.Isbn == isbn.Replace("'", "\'"))).Any();
        }

        //BOOK function
        public int AddBook(BookDTO book)
        {
            var author = _context.Author.Where((System.Linq.Expressions.Expression<Func<Author, bool>>)(a => a.AuthorFullname == book.AuthorFullname)).FirstOrDefault();

            if (author == null)
            {
                _context.Author.Add(new Author(book.AuthorFullname));
            }

            Book b = new Book(book.Title, book.Isbn, author.AuthorId, book.Year, book.Language, book.Type, book.Description, true);
            _context.Book.Add(b);
            _context.SaveChangesAsync();
            var volume = new Volume
            {
                IsFree = true,
                BookId = book.BookId
            };

            _context.Volume.Add(volume);
            _context.SaveChanges();
            return b.BookId;
        }

        public BookDTO GetBookById(int id)
        {
            Book book = _context.Book.Where(a => a.BookId == id).FirstOrDefault();
            if (book == null)
                return null;
            else
            {
                string authorFullname = _context.Author.Where(a => a.AuthorId == book.AuthorId).FirstOrDefault().AuthorFullname;
                return new BookDTO(book.BookId, book.Title, book.Isbn, authorFullname, book.Year, book.Language, book.Type, book.Description, book.IsAvailable);
            }
        }

        public List<VolumeDTO> GetVolumeByBookId(int id)
        {
            List<Volume> volumes = new List<Volume>(_context.Volume.Where(a => a.BookId == id));

            if (!volumes.Any())
            {
                return null;
            }

            return volumes.Select(volume => new VolumeDTO()
            {
                VolumeId = volume.VolumeId,
                BookId = volume.BookId,
                IsFree = volume.IsFree
            }).ToList();
        }

        public List<BookDTO> SearchBook(string[] search)
        {
            string[] name = { "bookId", "ISBN", "title", "author_id", "year", "language", "type" };
            var sql = "Select * from Book where isAvailable=true ";
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i] != "%")
                {
                    if (name[i] == "title")
                    {
                        string[] words = search[i].ToLower().Split(" ");
                        for (int j = 0; j < words.Length; j++)
                        {
                            sql += " and title like('%" + words[j].Replace("'", "\'") + "%') ";
                        }
                    }
                    else if (name[i] == "author_id")
                    {
                        int author = _context.Author.Where(a => a.AuthorFullname == search[i].Replace("'", "\'")).FirstOrDefault().AuthorId;
                        sql += " and author_id= " + author.ToString() + " ";
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i].Replace("'", "\'") + "'";
                    }
                }

            }
            List<Book> book_db = _context.Book.FromSql(sql).ToList();
            List<BookDTO> book_dto = new List<BookDTO>();
            string authorFullname;
            foreach (Book book in book_db)
            {
                authorFullname = _context.Author.Where(a => a.AuthorId == book.AuthorId).FirstOrDefault().AuthorFullname;
                book_dto.Add(new BookDTO(book.BookId, book.Title, book.Isbn, authorFullname, book.Year, book.Language, book.Type, book.Description, book.IsAvailable));
            }

            return book_dto;
        }

        public void RemoveBook(int bookId)
        {
            _context.Reservation.FromSql("DELETE from Reservation where bookId='" + bookId + "'");


            foreach (Volume volume in _context.Volume.Where(a => a.BookId == bookId))
            {
                _context.Volume.Remove(volume);
            }
            _context.Book.Find(bookId).IsAvailable = false;
            //usuń wszystkie rezerwacje
            _context.SaveChangesAsync();
        }

        public VolumeDTO AddVolume(int id)
        {
            var volume = new Volume()
            {
                IsFree = true,
                BookId = id
            };

            _context.Volume.Add(volume);
            _context.SaveChanges();

            return new VolumeDTO(volume.VolumeId, volume.BookId, volume.IsFree);
        }

        public string RemoveVolumeCheckCondition(int id)
        {
            if (!_context.Volume.Where(a => a.VolumeId == id).Any())
            {
                return "Egzemplarz o danym id nie istnieje!";
            }
            else
            {
                return "Dany egzemplarz jest wypożyczony. Nie można go usunąć!";
            }
        }

        // TODO
        public async Task<IActionResult> RemoveVolume(int id)
        {
            var volume = _context.Volume.Where(a => a.VolumeId == id).FirstOrDefault();
            if (_context.Reservation.Where(a => a.VolumeId == id && a.IsActive == true).Any())
            {
                Reservation reservation = _context.Reservation.Where(a => a.VolumeId == id && a.IsActive == true).FirstOrDefault();

                foreach (Reservation reserv in _context.Reservation.Where(a => a.BookId == volume.BookId && a.IsActive == false))
                {
                    reserv.Queue = reserv.Queue + 1;
                }
                if (_context.Volume.Where(a => a.BookId == volume.BookId).Count() > 1)
                {
                    var n = _context.Volume.Where(a => a.BookId == volume.BookId && a.VolumeId != id).FirstOrDefault();
                    Reservation r = new Reservation(reservation.UserId, reservation.BookId, n.VolumeId, reservation.StartDate, reservation.ExpireDate, 1, false);
                    r.ReservationId = reservation.ReservationId;
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

        public bool GetSuggestionCheckCondition(int userId)
        {
            return _context.User.Where(a => a.UserId == userId).Count() == 0;
        }

        public List<SuggestionDTO> GetSuggestion(int userId)
        {
            var sql = "CALL Get_suggestion(" + userId + ")";
            _context.Database.ExecuteSqlCommand(sql);
            List<Suggestion> suggestion = _context.Suggestion.ToList();
            List<SuggestionDTO> SuggestionDTO = new List<SuggestionDTO>();

            foreach (Suggestion sug in suggestion)
            {
                SuggestionDTO.Add(new SuggestionDTO(sug.Id, sug.Title, sug.AuthorFullname));
            }
            return (SuggestionDTO);
        }

        public bool BookExists(int id)
        {
            return _context.Book.Select(e => e.BookId == id).Any();
        }

        public void UpdateBook(BookDTO book)
        {
            if (_context.Author.Where(a => a.AuthorFullname == book.AuthorFullname).Count() == 0)
            {
                _context.Author.Add(new Author(book.AuthorFullname));
                _context.SaveChanges();
            }
            int author_id = _context.Author.Where(a => a.AuthorFullname == book.AuthorFullname).FirstOrDefault().AuthorId;

            Book book_dbo = new Book(book.BookId, book.Title, book.Isbn, author_id, book.Year, book.Language, book.Type, book.Description, book.IsAvailable);
            _context.Entry(book_dbo).State = EntityState.Modified;
            _context.Book.Update(book_dbo);

            _context.SaveChanges();
        }

        public async Task<IActionResult> EditBook(BookDTO book)
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
                if (_context.Book.Where(a => a.BookId == book.BookId).Count() == 0)
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