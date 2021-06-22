using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Reservation
    {
        public Reservation(int user_id, int book_id, int volume_id, DateTime start_date, DateTime expire_date, int queue, Boolean is_active)
        {
            this.user_id = user_id;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.expire_date = expire_date;
            this.queue = queue;
            this.is_active = is_active;
        }

        public Reservation(int reservation_id, int user_id, int book_id, int volume_id, DateTime start_date, DateTime expire_date, int queue, bool is_active)
        {
            this.reservation_id = reservation_id;
            this.user_id = user_id;
            this.book_id = book_id;
            this.volume_id = volume_id;
            this.start_date = start_date;
            this.expire_date = expire_date;
            this.queue = queue;
            this.is_active = is_active;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int reservation_id { get; set; }
        public int user_id { get; set; }
        public int book_id { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime expire_date { get; set; }
        public Int32 queue { get; set; }
        public Boolean is_active { get; set; }
    }
}