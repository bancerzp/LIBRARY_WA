using System;

namespace LIBRARY_WA.Models
{
    public class Rent_DTO
    {
        public int RentId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public Rent_DTO(int userId, int bookId, string title, string isbn, int volumeId, DateTime startDate, DateTime expireDate)
        {
            UserId = userId;
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
        }

        public Rent_DTO(int rentId, int userId, int bookId, string title, string isbn, int volumeId, DateTime startDate, DateTime expireDate)
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