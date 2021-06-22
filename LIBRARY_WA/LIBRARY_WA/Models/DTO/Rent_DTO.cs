﻿using System;

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

        public Rent_DTO(int rent_id, int user_id, int book_id, string title, string isbn, int volume_id, DateTime start_date, DateTime expire_date)
        {
            this.rent_id = rent_id;
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
        public string title { get; set; }
        public string isbn { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime expire_date { get; set; }
    }
}