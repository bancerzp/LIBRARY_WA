using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace LIBRARY_WA.Controllers.Services
{
  
    public class BookService : ControllerBase
    {
        private readonly LibraryContext _context;

        public BookService(LibraryContext context)
        {
            _context = context;
        }

        // get data to combobox

        public List<String> GetAuthor()
        {
            return _context.Book.Where(a => a.is_available == true).Select(a => a.author_fullname).Distinct().ToList();
        }


        public List<String> GetBookType()
        {
            return _context.Book.Where(a => a.is_available == true).Select(a => a.type).Distinct().ToList();
        }


        public List<String> GetLanguage()
        {
            return _context.Book.Where(a => a.is_available == true).Select(a => a.language).Distinct().ToList();
        }


        public bool IfISBNExists( String isbn)
        {
            return _context.Book.Where(a => (a.isbn == isbn.Replace("'", "\'"))).Count()>0;
        }



        //BOOK function


        public int AddBook(Book_DTO book)
        {
            int author_id = _context.Author.Where(a => a.author_fullname == book.author_fullname);
            Book b = new Book( book.title, book.isbn, author_id, book.year, book.language, book.type, book.description, true);
            _context.Book.Add(b);
            _context.SaveChangesAsync();
            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = book.book_id;
            _context.Volume.Add(volume);
            _context.SaveChanges();
            return b.book_id;
        }


        public Book_DTO GetBookById(int id)
        {
            Book book = _context.Book.Find(id);
            if (book == null)
                return null;
            else { 
            String author_fullname = _context.Author.Find(book.author_id);
            return new Book_DTO(book.book_id,book.title,book.isbn,author_fullname,book.year,book.language,book.type,book.description,book.is_available);
            }
        }


        public Volume_DTO GetVolumeByBookId( int id)
        {
            Volume volume = _context.Volume.Where(a => a.book_id == id).FirstOrDefault();

            if (volume == null)
            {
                return null;
            }

            return new Volume_DTO(volume.volume_id, volume.book_id, volume.is_free);
        }



        public  List<Book_DTO> SearchBook( String[] search)
        {
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
                            sql += " and title like('%" + words[j].Replace("'", "\'") + "%') ";
                        }
                    }
                    else
                    {
                        sql += "and " + name[i] + "='" + search[i].Replace("'", "\'") + "'";
                    }
                }

            }
            List<Book> book_db = _context.Book.FromSql(sql).ToList();
            List<Book_DTO> book_dto = new List<Book_DTO>();
            foreach(Book book in book_db)
            {
                String author_fullname = _context.Author.Find(book.author_id);
                book_dto.Add(new Book_DTO(book.book_id, book.title, book.isbn, author_fullname, book.year, book.language, book.type, book.description, book.is_available);
            }
        
            return book_dto;
        }
        public Boolean GetRentById(int id)
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
        
        //Volume function

        public Volume_DTO  AddVolume( int id)
        {
            Volume volume = new Volume();
            volume.is_free = true;
            volume.book_id = id;
            _context.Volume.Add(volume);
            _context.SaveChanges();
            return new Volume_DTO(volume.volume_id,volume.book_id,volume.is_free);
        }

        public Volume_DTO GetVolumeIn(int id,String cond)
        {
            Volume volume = _context.Volume.Where(a => a.book_id == id).FirstOrDefault();
            switch(cond)
            case ("volume"):
                break;
            case ("reservation"):
                break
            if (volume == null)
            {
                return null;
            }

            return new Volume_DTO(volume.volume_id, volume.book_id, volume.is_free);
        }


        public async Task<IActionResult> RemoveVolume( int id)
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



        public async Task<IActionResult> ReserveBook(int[] data)
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

            if (_context.Reservation.Where(a => a.book_id == data[0] && a.user_id == data[1]).Count() > 0)
            {
                return BadRequest(new { alert = "Użytkownik ma już zarezerwowaną tę książkę!" });
            }

            if (_context.Volume.Where(a => a.book_id == data[0]).Count() == 0)
            {
                return NotFound(new { alert = "Książka nie ma żadnych egzemplarzy!" });
            }

            _Book book = _context.Book.Where(a => a.book_id == (data[0])).FirstOrDefault();

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




        public async Task<IActionResult> RentBook(int[] reservation_id)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Reservation.Where(a => a.reservation_id == reservation_id[0]).Count() == 0)
            {
                return BadRequest(new { alert = "Nie ma takiej rezerwacji" });
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


        public async Task<IActionResult> ReturnBook( int[] rent_id)
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


        public async Task<IActionResult> GetSuggestion( int user_id)
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


        public async Task<IActionResult> UpdateBook( Book book)
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







        public async Task<IActionResult> EditBook( Book book)
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



