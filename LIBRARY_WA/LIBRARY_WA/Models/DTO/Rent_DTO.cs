using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Rent_DTO
    {
        public Rent_DTO(int user_id, int book_id, string title, string isbn, int volume_id, DateTime start_date, DateTime expire_date)
        {
           
            this.user_id = user_id;
            this.book_id = book_id;
            this.title = title;
            this.isbn = isbn;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.expire_date = expire_date;
        }

       
        public int rent_id { get; set; }
        public int user_id { get; set; }
        public int book_id { get; set; }
        public String title { get; set; }
        public String isbn { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime expire_date { get; set; }

    }
}
