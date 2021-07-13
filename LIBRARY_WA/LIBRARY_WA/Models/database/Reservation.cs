using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class Reservation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Queue { get; set; }
        public bool IsActive { get; set; }

        public Reservation(int userId, int bookId, int volumeId, DateTime startDate, DateTime expireDate, int queue, bool isActive)
        {
            UserId = userId;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
            Queue = queue;
            IsActive = isActive;
        }

        public Reservation(int reservationId, int userId, int bookId, int volumeId, DateTime startDate, DateTime expireDate, int queue, bool isActive)
        {
            ReservationId = reservationId;
            UserId = userId;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
            Queue = queue;
            IsActive = isActive;
        }
    }
}