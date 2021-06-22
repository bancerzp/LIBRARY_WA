using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Volume
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int volume_id { get; set; }
        public int book_id { get; set; }
        public Boolean is_free { get; set; }

        public Volume(int book_id, bool is_free)
        {
            this.book_id = book_id;
            this.is_free = is_free;
        }

        public Volume()
        {
        }
    }
}
