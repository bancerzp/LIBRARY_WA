using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Reservation
    {
        [Key]
        public Int32 reservation_id { get; set; }
        public Int32 user_id { get; set; }
        [MaxLength(50)]
        public String title { get; set; }

        [StringLength(13)]
        public String isbn { get; set; }
        public Int32 book_id { get; set; }
        public Int32 volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime expire_date { get; set; }
        public Boolean is_active { get; set; }
    }
}
