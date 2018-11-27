using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models
{
    public class Rent
    {
        [Key]
        public Int32 rent_id { get; set; }

        public Int32 user_id { get; set; }

        public Int32 book_id { get; set; }
        [MaxLength(50)]
        public String title { get; set; }

        [StringLength(13)]
        public String isbn { get; set; }

        public Int32 volume_id { get; set; }

        public DateTime start_date { get; set; }

        public DateTime expire_date { get; set; }

    }
}
