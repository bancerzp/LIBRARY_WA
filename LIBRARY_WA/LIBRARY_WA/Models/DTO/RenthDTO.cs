using System;

namespace Library.Models
{
    public class RenthDTO
    {
        public int RentIdH { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public int BookId { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public RenthDTO( int userId, string title, string isbn, int bookId, int volumeId, DateTime startDate, DateTime endDate)
        {
            UserId = userId;
            Title = title;
            Isbn = isbn;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            EndDate = endDate;
        }

        public RenthDTO(int rentIdH,int userId, string title, string isbn, int bookId, int volumeId, DateTime startDate, DateTime endDate)
        {
            RentIdH = rentIdH;
            UserId = userId;
            Title = title;
            Isbn = isbn;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}