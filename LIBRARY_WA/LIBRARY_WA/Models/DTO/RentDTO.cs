using System;

namespace Library.Models
{
    public class RentDTO
    {
        public int RentId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public RentDTO(int userId, int bookId, string title, string isbn, int volumeId, DateTime startDate, DateTime expireDate)
        {
            UserId = userId;
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
        }

        public RentDTO(int rentId, int userId, int bookId, string title, string isbn, int volumeId, DateTime startDate, DateTime expireDate)
        {
            RentId = rentId;
            UserId = userId;
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
        }
    }
}