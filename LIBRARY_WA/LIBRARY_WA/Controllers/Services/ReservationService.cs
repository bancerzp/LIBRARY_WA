using LIBRARY_WA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace LIBRARY_WA.Controllers.Services
{
    public class ReservationService : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly LibraryContext _context;

        public ReservationService(LibraryContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public ActionResult<ReservationDTO> ReserveBook(int[] data)
        {

            Book book = _context.Book.Where(a => a.BookId == (data[0])).FirstOrDefault();

            int volumeId = _context.Volume.Where(a => a.BookId == data[0]).FirstOrDefault().VolumeId;

            var startDate = DateTime.Now;
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
            return new ReservationDTO(reservation.UserId, b.Title, b.Isbn, b.BookId, reservation.VolumeId, reservation.StartDate, reservation.ExpireDate, reservation.Queue + 1, reservation.IsActive);
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

            if (_context.Reservation.Where(a => a.BookId == data[0] && a.UserId == data[1]).Any())
            {
                return "Użytkownik ma już zarezerwowaną tę książkę!";
            }

            if (_context.Volume.Where(a => a.BookId == data[0]).Count() == 0)
            {
                return "Książka nie ma żadnych egzemplarzy!";
            }

            return "";
        }
    }
}