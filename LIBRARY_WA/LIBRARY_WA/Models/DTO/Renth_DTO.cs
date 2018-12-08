using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Renth_DTO
    {
        public Renth_DTO( int user_id, string title, string isbn, int book_id, int volume_id, DateTime start_date, DateTime end_date)
        {
            this.user_id = user_id;
            this.title = title;
            this.isbn = isbn;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.end_date = end_date;
        }

        public Renth_DTO(int rent_id_h,int user_id, string title, string isbn, int book_id, int volume_id, DateTime start_date, DateTime end_date)
        {
            this.rent_id_h = rent_id_h;
            this.user_id = user_id;
            this.title = title;
            this.isbn = isbn;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.end_date = end_date;
        }


        public int rent_id_h { get; set; }
        public int user_id { get; set; }
        public String title { get; set; }        
        public String isbn { get; set; }
        public int book_id { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
