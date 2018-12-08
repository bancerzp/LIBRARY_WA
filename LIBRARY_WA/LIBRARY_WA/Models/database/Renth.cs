using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Renth
    {
        public Renth( int user_id, int book_id, int volume_id, DateTime start_date, DateTime end_date)
        {
            this.user_id = user_id;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.end_date = end_date;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rent_id_h { get; set; }
        public int user_id { get; set; }
        public int book_id { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
