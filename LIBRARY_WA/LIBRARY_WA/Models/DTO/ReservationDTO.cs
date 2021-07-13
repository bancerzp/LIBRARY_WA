using System;

namespace Library.Models
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }

        public string Isbn { get; set; }
        public int BookId { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Queue { get; set; }
        public bool IsActive { get; set; }

        public ReservationDTO(int userId,string title,string isbn, int bookId, int volumeId, DateTime startDate, DateTime expireDate, int queue,bool isActive)
        {
            UserId = userId;
            Isbn = isbn;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
            Queue = queue;
            IsActive = isActive;
            Title = title;
        }

        public ReservationDTO(int reservationId, int userId, string title, string isbn, int bookId, int volumeId, DateTime startDate, DateTime expireDate, int queue, bool isActive)
        {
            ReservationId = reservationId;
            UserId = userId;
            Title = title;
            Isbn = isbn;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
            Queue = queue;
            IsActive = isActive;
        }
    }
}