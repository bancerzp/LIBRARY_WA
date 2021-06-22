using System;

namespace LIBRARY_WA.Models
{
    public class Reservation_DTO
    {
        public Reservation_DTO(int user_id,string title,string isbn, int book_id, int volume_id, DateTime start_date, DateTime expire_date, int queue,bool is_active)
        {
            this.user_id = user_id;
            this.isbn = isbn;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.expire_date = expire_date;
            this.queue = queue;
            this.is_active = is_active;
            this.title = title;
        }

        public Reservation_DTO(int reservation_id, int user_id, string title, string isbn, int book_id, int volume_id, DateTime start_date, DateTime expire_date, int queue, bool is_active)
        {
            this.reservation_id = reservation_id;
            this.user_id = user_id;
            this.title = title;
            this.isbn = isbn;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.expire_date = expire_date;
            this.queue = queue;
            this.is_active = is_active;
        }

        public int reservation_id { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }

        public string isbn { get; set; }
        public int book_id { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime expire_date { get; set; }
        public Int32 queue { get; set; }
        public bool is_active { get; set; }

    }
}