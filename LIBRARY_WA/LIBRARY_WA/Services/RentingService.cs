using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Services
{
    public class RentingService : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly LibraryContext _context;

        public RentingService(LibraryContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        public string RentBookCheckCondition(int[] reservationId)
        {

            if (_context.Reservation.Where(a => a.ReservationId == reservationId[0]).Count() == 0)
            {
                return "Nie ma takiej rezerwacji";
            }

            var reservation = _context.Reservation.Where(a => a.ReservationId == reservationId[0]).FirstOrDefault();
            if (_context.Volume.Where(a => a.IsFree == true).Count() == 0)
            {
                return "Nie ma takiego egzemplarza";
            }
            return "";
        }

        public ActionResult<RentDTO> RentBook(int[] reservationId)
        {
            var reservation = _context.Reservation.Where(a => a.ReservationId == reservationId[0]).FirstOrDefault();

            int volumeId = _context.Volume.Where(a => a.IsFree == true).FirstOrDefault().VolumeId;
            //zmień is free na false
            _context.Volume.Where(a => a.IsFree == true).FirstOrDefault().IsFree = false;
            Rent rent = new Rent(reservation.UserId, reservation.BookId, volumeId, DateTime.Now, DateTime.Now.AddMonths(1));
            _context.Rent.Add(rent);
            _context.Reservation.Remove(reservation);
            _context.SaveChanges();
            Book book = _context.Book.Where(a => a.BookId == reservation.BookId).FirstOrDefault();
            return Ok(new RentDTO(reservation.UserId, book.BookId, book.Title, book.Isbn, reservation.VolumeId, reservation.StartDate, reservation.ExpireDate));
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

        public bool IsUserBlocked(int reservationId)
        {
            Reservation reservation = _context.Reservation.Where(a => a.ReservationId == reservationId).SingleOrDefault();
            return _context.User.Where(a => a.UserId == reservation.UserId).SingleOrDefault().IsValid;
        }

        public bool IsRentExist(int rentId)
        {
            return _context.Rent.Where(a => a.BookId == rentId).Any();
        }
    }
}