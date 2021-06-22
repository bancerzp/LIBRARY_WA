using System;

namespace LIBRARY_WA.Models
{
    public class Volume_DTO
    {
        public int volume_id { get; set; }
        public int book_id { get; set; }
        public bool is_free { get; set; }

        public Volume_DTO(int book_id, bool is_free)
        {
            this.book_id = book_id;
            this.is_free = is_free;
        }

        public Volume_DTO(int volume_id,int book_id, bool is_free)
        {
            this.volume_id = volume_id;
            this.book_id = book_id;
            this.is_free = is_free;
        }

        public Volume_DTO()
        {
        }
    }
}