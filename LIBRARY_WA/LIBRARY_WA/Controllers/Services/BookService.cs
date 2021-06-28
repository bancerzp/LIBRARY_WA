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
        public List<string> GetAuthorsFullname()
        {
            return _context.Author.Select(a => a.AuthorFullname).ToList();
        }

        public List<string> GetBookTypes()
        {
            return _context.Book.Select(a => a.Type).Distinct().ToList();
        }

        public List<string> GetLanguages()
        {
            return _context.Book.Select(a => a.Language).Distinct().ToList();
        }

        public bool IfISBNExists(string isbn)
        {
            return _context.Book.Where(a => (a.Isbn == isbn.Replace("'", "\'"))).Count() > 0;
        }

        public bool IsBlocked(int reservationId)
        {
            Reservation reservation = _context.Reservation.Where(a => a.ReservationId == reservationId).SingleOrDefault();
            return _context.User.Where(a => a.UserId == reservation.UserId).SingleOrDefault().IsValid;
        }

        //BOOK function
        public int AddBook(Book_DTO book)
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

        public Book_DTO GetBookById(int id)
        {
            Book book = _context.Book.Where(a => a.BookId == id).FirstOrDefault();
            if (book == null)
                return null;
            else
            {
                string authorFullname = _context.Author.Where(a => a.AuthorId == book.AuthorId).FirstOrDefault().AuthorFullname;
                return new Book_DTO(book.BookId, book.Title, book.Isbn, authorFullname, book.Year, book.Language, book.Type, book.Description, book.IsAvailable);
            }
        }

        public List<Volume_DTO> GetVolumeByBookId(int id)
        {
            List<Volume> volumes = new List<Volume>(_context.Volume.Where(a => a.BookId == id));

            if (!volumes.Any())
            {
                return null;
            }

            return volumes.Select(volume => new Volume_DTO()
            {
                VolumeId = volume.VolumeId,
                BookId = volume.BookId,
                IsFree = volume.IsFree
            }).ToList();
        }

        public List<Book_DTO> SearchBook(string[] search)
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
            List<Book_DTO> book_dto = new List<Book_DTO>();
            string authorFullname;
            foreach (Book book in book_db)
            {
                authorFullname = _context.Author.Where(a => a.AuthorId == book.AuthorId).FirstOrDefault().AuthorFullname;
                book_dto.Add(new Book_DTO(book.BookId, book.Title, book.Isbn, authorFullname, book.Year, book.Language, book.Type, book.Description, book.IsAvailable));
            }

            return book_dto;
        }

        public bool IsRentExist(int rentId)
        {
            return _context.Rent.Where(a => a.BookId == rentId).Count() > 0;
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

        public Volume_DTO AddVolume(int id)
        {
            var volume = new Volume()
            {
                IsFree = true,
                BookId = id
            };

            _context.Volume.Add(volume);
            _context.SaveChanges();

            return new Volume_DTO(volume.VolumeId, volume.BookId, volume.IsFree);
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
            if (_context.Reservation.Where(a => a.VolumeId == id && a.IsActive == true).Count() > 0)
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

        public string ReserveBookCheckCondition(int[] data)
        {

            if (_context.Book.Where(a => a.BookId == data[0]).Count() == 0)
            {
                return "Nie znaleziono książki o podanym id";
            }

            if (_context.User.Where(a => a.UserId == (data[1])).Count() == 0)
            {
                return "Nie znaleziono użytkownika o podanym id";
            }

            if (_context.Reservation.Where(a => a.BookId == data[0] && a.UserId == data[1]).Count() > 0)
            {
                return "Użytkownik ma już zarezerwowaną tę książkę!";
            }

            if (_context.Volume.Where(a => a.BookId == data[0]).Count() == 0)
            {
                return "Książka nie ma żadnych egzemplarzy!";
            }

            return "";
        }

        //TODO
        public ActionResult<Reservation_DTO> ReserveBook(int[] data)
        {

            Book book = _context.Book.Where(a => a.BookId == (data[0])).FirstOrDefault();

            int volumeId = _context.Volume.Where(a => a.BookId == data[0]).FirstOrDefault().VolumeId;

            DateTime startDate = DateTime.Now;
            DateTime expireDate;
            int queue;
            var isActive = true;


            if (_context.Volume.Where(a => a.BookId == data[0] && a.IsFree == true).Count() == 0)
            {
                if (_context.Reservation.Where(a => a.BookId == data[0]).OrderByDescending(a => a.Queue).FirstOrDefault() == null)
                    queue = 0;
                else
                    queue = _context.Reservation.Where(a => a.BookId == data[0]).OrderByDescending(a => a.Queue).FirstOrDefault().Queue + 1;
                isActive = false;
                expireDate = DateTime.Now;
            }
            else
            {
                volumeId = _context.Volume.Where(a => a.BookId == Convert.ToInt32(data[0]) && a.IsFree == true).FirstOrDefault().VolumeId;
                expireDate = DateTime.Now.AddDays(14);
                _context.Volume.Where(a => a.VolumeId == volumeId).FirstOrDefault().IsFree = false;
                queue = 0;

            }

            Reservation reservation = new Reservation(data[1], book.BookId, volumeId, startDate, expireDate, queue, isActive);

            _context.Reservation.Add(reservation);
            _context.SaveChanges();


            Book b = _context.Book.Where(a => a.BookId == reservation.BookId).FirstOrDefault();
            return new Reservation_DTO(reservation.UserId, b.Title, b.Isbn, b.BookId, reservation.VolumeId, reservation.StartDate, reservation.ExpireDate, reservation.Queue + 1, reservation.IsActive);
        }



        public string RentBookCheckCondition(int[] reservationId)
        {

            if (_context.Reservation.Where(a => a.ReservationId == reservationId[0]).Count() == 0)
            {
                return "Nie ma takiej rezerwacji";
            }

            Reservation reservation = _context.Reservation.Where(a => a.ReservationId == reservationId[0]).FirstOrDefault();
            if (_context.Volume.Where(a => a.IsFree == true).Count() == 0)
            {
                return "Nie ma takiego egzemplarza";
            }
            return "";
        }
        public ActionResult<Rent_DTO> RentBook(int[] reservationId)
        {

            Reservation reservation = _context.Reservation.Where(a => a.ReservationId == reservationId[0]).FirstOrDefault();

            int volumeId = _context.Volume.Where(a => a.IsFree == true).FirstOrDefault().VolumeId;
            //zmień is free na false
            _context.Volume.Where(a => a.IsFree == true).FirstOrDefault().IsFree = false;
            Rent rent = new Rent(reservation.UserId, reservation.BookId, volumeId, DateTime.Now, DateTime.Now.AddMonths(1));
            _context.Rent.Add(rent);
            _context.Reservation.Remove(reservation);
            _context.SaveChanges();
            Book book = _context.Book.Where(a => a.BookId == reservation.BookId).FirstOrDefault();
            return Ok(new Rent_DTO(reservation.UserId, book.BookId, book.Title, book.Isbn, reservation.VolumeId, reservation.StartDate, reservation.ExpireDate));
        }

        public bool ReturnBookCheckCondition(int[] rentId)
        {
            return _context.Rent.Where(a => a.RentId == rentId[0]).Count() == 0;
        }

        public async Task<IActionResult> ReturnBook(int[] rentId)
        {
            Rent rent = _context.Rent.Where(a => a.RentId == rentId[0]).FirstOrDefault();
            Volume volume = _context.Volume.Where(a => a.VolumeId == rent.VolumeId).FirstOrDefault();
            Reservation[] reservations = _context.Reservation.Where(a => a.BookId == rent.BookId).ToArray();
            Renth renth = new Renth(rent.UserId, rent.BookId, rent.VolumeId, rent.StartDate, DateTime.Now);
            //wstaw do historii rezerwacji
            await _context.Renth.AddAsync(renth);
            await _context.SaveChangesAsync();

            foreach (Reservation reservation in reservations)
            {
                reservation.Queue = reservation.Queue - 1;
                if (reservation.Queue == 0)
                {
                    reservation.IsActive = true;
                    reservation.ExpireDate = DateTime.Now.AddDays(8);
                    reservation.VolumeId = volume.VolumeId;
                }
            }

            //usuń z rent
            _context.Rent.Remove(rent);
            _context.SaveChanges();
            return Ok();
        }

        public bool GetSuggestionCheckCondition(int userId)
        {
            return _context.User.Where(a => a.UserId == userId).Count() == 0;
        }

        public List<Suggestion_DTO> GetSuggestion(int userId)
        {
            var sql = "CALL Get_suggestion(" + userId + ")";
            _context.Database.ExecuteSqlCommand(sql);
            List<Suggestion> suggestion = _context.Suggestion.ToList();
            List<Suggestion_DTO> suggestion_dto = new List<Suggestion_DTO>();

            foreach (Suggestion sug in suggestion)
            {
                suggestion_dto.Add(new Suggestion_DTO(sug.Id, sug.Title, sug.AuthorFullname));
            }
            return (suggestion_dto);
        }

        public bool BookExists(int id)
        {
            return _context.Book.Select(e => e.BookId == id).Count() > 0;
        }

        public void UpdateBook(Book_DTO book)
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