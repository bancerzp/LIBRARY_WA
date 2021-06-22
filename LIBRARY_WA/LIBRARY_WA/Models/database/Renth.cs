using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Renth
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RentIdH { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int VolumeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Renth(int userId, int bookId, int volumeId, DateTime startDate, DateTime endDate)
        {
            UserId = userId;
            BookId = bookId;
            VolumeId = volumeId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}