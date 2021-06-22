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

        //TODO
        public List<String> GetAuthor()
        {
            return _context.Author.Select(a => a.author_fullname).Distinct().ToList();
        }

        public List<String> GetBookType()
        {
            return _context.Book.Select(a => a.type).Distinct().ToList();
        }

        public List<String> GetLanguage()
        {
            return _context.Book.Select(a => a.language).Distinct().ToList();
        }

        public bool IfISBNExists(string isbn)
        {
            return _context.Book.Where(a => (a.isbn == isbn.Replace("'", "\'"))).Count() > 0;
        }

        public bool IsBlocked(int reservation_id)
        {
            Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservation_id).SingleOrDefault();
            return _context.User.Where(a => a.user_id == reservation.user_id).SingleOrDefault().is_valid;
        }

        //BOOK function
        public int AddBook(Book_DTO book)
        {
            var author = _context.Author.Where((System.Linq.Expressions.Expression<Func<Author, bool>>)(a => a.author_fullname == book.author_fullname)).FirstOrDefault();

            if (author == null)
            {
                _context.Author.Add(new Author((string)book.author_fullname));
            }
            Book b = new Book(book.title, book.isbn, author.author_id, book.year, book.language, book.type, book.description, true);
            _context.Book.Add(b);
            _context.SaveChangesAsync();
            var volume = new Volume
            {
                is_free = true,
                book_id = book.book_id
            };

            _context.Volume.Add(volume);
            _context.SaveChanges();
            return b.book_id;
        }

        public Book_DTO GetBookById(int id)
        {
            Book book = _context.Book.Where(a => a.book_id == id).FirstOrDefault();
            if (book == null)
                return null;
            else
            {
                string author_fullname = _context.Author.Where(a => a.author_id == book.author_id).FirstOrDefault().author_fullname;
                return new Book_DTO(book.book_id, book.title, book.isbn, author_fullname, book.year, book.language, book.type, book.description, book.is_available);
            }
        }

        public List<Volume_DTO> GetVolumeByBookId(int id)
        {
            List<Volume> volumes = new List<Volume>(_context.Volume.Where(a => a.book_id == id));
            List<Volume_DTO> volumes_dto = new List<Volume_DTO>();

            if (volumes.Count == 0)
            {
                return null;
            }

            foreach (Volume v in volumes)
            {
                volumes_dto.Add(new Volume_DTO(v.volume_id, v.book_id, v.is_free));
            }

            return volumes_dto;
        }

        public List<Book_DTO> SearchBook(string[] search)
        {
            string[] name = { "book_id", "ISBN", "title", "author_id", "year", "language", "type" };
            var sql = "Select * from Book where is_available=true ";
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
                        int author = _context.Author.Where(a => a.author_fullname == search[i].Replace("'", "\'")).FirstOrDefault().author_id;
                        sql += " and author_id= " + author.ToString() + " ";
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i].Replace("'", "\'") + "'";
                    }
                }

            }
            List<Book> book_db = _context.Book.FromSql(sql).ToList();
            List<Book_DTO> book_dto = new List<Book_DTO>();
            string author_fullname;
            foreach (Book book in book_db)
            {
                author_fullname = _context.Author.Where(a => a.author_id == book.author_id).FirstOrDefault().author_fullname;
                book_dto.Add(new Book_DTO(book.book_id, book.title, book.isbn, author_fullname, book.year, book.language, book.type, book.description, book.is_available));
            }

            return book_dto;
        }
        public bool GetRentById(int id)
        {
            return _context.Rent.Where(a => a.book_id == id).Count() > 0;
        }

        public void RemoveBook(int id)
        {
            _context.Reservation.FromSql("DELETE from Reservation where book_id='" + id + "'");


            foreach (Volume volume in _context.Volume.Where(a => a.book_id == id))
            {
                _context.Volume.Remove(volume);
            }
            _context.Book.Find(id).is_available = false;
            //usuń wszystkie rezerwacje
            _context.SaveChangesAsync();
        }

        public Volume_DTO AddVolume(int id)
        {
            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = id;
            _context.Volume.Add(volume);
            _context.SaveChanges();
            return new Volume_DTO(volume.volume_id, volume.book_id, volume.is_free);
        }

        public string RemoveVolumeCheckCondition(int id)
        {
            if (_context.Volume.Where(a => a.volume_id == id).Count() == 0)
            {
                return "Egzemplarz o danym id nie istnieje!";
            }

            var volume = _context.Volume.Where(a => a.volume_id == id).FirstOrDefault();

            if (_context.Rent.Where(a => a.volume_id == id).Count() > 0)
            {
                return "Dany egzemplarz jest wypożyczony. Nie można go usunąć!";
            }
            return "";
        }

        // TODO
        public async Task<IActionResult> RemoveVolume(int id)
        {
            var volume = _context.Volume.Where(a => a.volume_id == id).FirstOrDefault();
            if (_context.Reservation.Where(a => a.volume_id == id && a.is_active == true).Count() > 0)
            {
                Reservation reservation = _context.Reservation.Where(a => a.volume_id == id && a.is_active == true).FirstOrDefault();

                foreach (Reservation reserv in _context.Reservation.Where(a => a.book_id == volume.book_id && a.is_active == false))
                {
                    reserv.queue = reserv.queue + 1;
                }
                if (_context.Volume.Where(a => a.book_id == volume.book_id).Count() > 1)
                {
                    var n = _context.Volume.Where(a => a.book_id == volume.book_id && a.volume_id != id).FirstOrDefault();
                    Reservation r = new Reservation(reservation.user_id, reservation.book_id, n.volume_id, reservation.start_date, reservation.expire_date, 1, false);
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

        public string ReserveBookCheckCondition(int[] data)
        {

            if (_context.Book.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return "Nie znaleziono książki o podanym id";
            }

            if (_context.User.Where(a => a.user_id == (data[1])).Count() == 0)
            {
                return "Nie znaleziono użytkownika o podanym id";
            }

            if (_context.Reservation.Where(a => a.book_id == data[0] && a.user_id == data[1]).Count() > 0)
            {
                return "Użytkownik ma już zarezerwowaną tę książkę!";
            }

            if (_context.Volume.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return "Książka nie ma żadnych egzemplarzy!";
            }

            return "";
        }

        //TODO
        public ActionResult<Reservation_DTO> ReserveBook(int[] data)
        {

            Book book = _context.Book.Where(a => a.book_id == (data[0])).FirstOrDefault();

            int volume_id = _context.Volume.Where(a => a.book_id == data[0]).FirstOrDefault().volume_id;

            DateTime start_date = DateTime.Now;
            DateTime expire_date;
            int queue;
            var is_active = true;


            if (_context.Volume.Where(a => a.book_id == data[0] && a.is_free == true).Count() == 0)
            {
                if (_context.Reservation.Where(a => a.book_id == data[0]).OrderByDescending(a => a.queue).FirstOrDefault() == null)
                    queue = 0;
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

            Reservation reservation = new Reservation(data[1], book.book_id, volume_id, start_date, expire_date, queue, is_active);

            _context.Reservation.Add(reservation);
            _context.SaveChanges();


            Book b = _context.Book.Where(a => a.book_id == reservation.book_id).FirstOrDefault();
            return new Reservation_DTO(reservation.user_id, b.title, b.isbn, b.book_id, reservation.volume_id, reservation.start_date, reservation.expire_date, reservation.queue + 1, reservation.is_active);
        }



        public string RentBookCheckCondition(int[] reservation_id)
        {

            if (_context.Reservation.Where(a => a.reservation_id == reservation_id[0]).Count() == 0)
            {
                return "Nie ma takiej rezerwacji";
            }

            Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservation_id[0]).FirstOrDefault();
            if (_context.Volume.Where(a => a.is_free == true).Count() == 0)
            {
                return "Nie ma takiego egzemplarza";
            }
            return "";
        }
        public ActionResult<Rent_DTO> RentBook(int[] reservation_id)
        {

            Reservation reservation = _context.Reservation.Where(a => a.reservation_id == reservation_id[0]).FirstOrDefault();

            int volume_id = _context.Volume.Where(a => a.is_free == true).FirstOrDefault().volume_id;
            //zmień is free na false
            _context.Volume.Where(a => a.is_free == true).FirstOrDefault().is_free = false;
            Rent rent = new Rent(reservation.user_id, reservation.book_id, volume_id, DateTime.Now, DateTime.Now.AddMonths(1));
            _context.Rent.Add(rent);
            _context.Reservation.Remove(reservation);
            _context.SaveChanges();
            Book book = _context.Book.Where(a => a.book_id == reservation.book_id).FirstOrDefault();
            return Ok(new Rent_DTO(reservation.user_id, book.book_id, book.title, book.isbn, reservation.volume_id, reservation.start_date, reservation.expire_date));
        }

        public bool ReturnBookCheckCondition(int[] rent_id)
        {
            return _context.Rent.Where(a => a.rent_id == rent_id[0]).Count() == 0;
        }

        public async Task<IActionResult> ReturnBook(int[] rent_id)
        {
            Rent rent = _context.Rent.Where(a => a.rent_id == rent_id[0]).FirstOrDefault();
            Volume volume = _context.Volume.Where(a => a.volume_id == rent.volume_id).FirstOrDefault();
            Reservation[] reservations = _context.Reservation.Where(a => a.book_id == rent.book_id).ToArray();
            Renth renth = new Renth(rent.user_id, rent.book_id, rent.volume_id, rent.start_date, DateTime.Now);
            //wstaw do historii rezerwacji
            await _context.Renth.AddAsync(renth);
            await _context.SaveChangesAsync();

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
            return Ok();
        }

        public bool GetSuggestionCheckCondition(int user_id)
        {
            return _context.User.Where(a => a.user_id == user_id).Count() == 0;
        }

        public List<Suggestion_DTO> GetSuggestion(int user_id)
        {
            var sql = "CALL Get_suggestion(" + user_id + ")";
            _context.Database.ExecuteSqlCommand(sql);
            List<Suggestion> suggestion = _context.Suggestion.ToList();
            List<Suggestion_DTO> suggestion_dto = new List<Suggestion_DTO>();

            foreach (Suggestion sug in suggestion)
            {
                suggestion_dto.Add(new Suggestion_DTO(sug.id, sug.title, sug.author_fullname));
            }
            return (suggestion_dto);
        }

        public bool BookExists(int id)
        {
            return _context.Book.Select(e => e.book_id == id).Count() > 0;
        }

        public void UpdateBook(Book_DTO book)
        {
            if (_context.Author.Where(a => a.author_fullname == book.author_fullname).Count() == 0)
            {
                _context.Author.Add(new Author(book.author_fullname));
                _context.SaveChanges();
            }
            int author_id = _context.Author.Where(a => a.author_fullname == book.author_fullname).FirstOrDefault().author_id;

            Book book_dbo = new Book(book.book_id, book.title, book.isbn, author_id, book.year, book.language, book.type, book.description, book.is_available);
            _context.Entry(book_dbo).State = EntityState.Modified;
            _context.Book.Update(book_dbo);

            _context.SaveChanges();
        }

        public async Task<IActionResult> EditBook(Book_DTO book)
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
                if (_context.Book.Where(a => a.book_id == book.book_id).Count() == 0)
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