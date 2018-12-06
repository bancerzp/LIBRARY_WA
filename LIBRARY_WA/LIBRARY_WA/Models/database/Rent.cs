using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Rent
    {
        public Rent(int user_id, int book_id, string title, string isbn, int volume_id, DateTime start_date, DateTime expire_date)
        {
           
            this.user_id = user_id;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.expire_date = expire_date;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rent_id { get; set; }

        public int user_id { get; set; }

        public int book_id { get; set; }

        [StringLength(13)]
        public String isbn { get; set; }

        public int volume_id { get; set; }

        public DateTime start_date { get; set; }

        public DateTime expire_date { get; set; }

    }
}
