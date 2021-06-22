using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Rent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RentId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public Rent(int userId, int bookId, int volumeId, DateTime startDate, DateTime expireDate)
        {
            UserId = userId;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            ExpireDate = expireDate;
        }
    }
}