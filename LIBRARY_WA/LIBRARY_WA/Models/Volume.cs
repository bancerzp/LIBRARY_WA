using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Volume
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int volume_id { get; set; }
        public int book_id { get; set; }
        public Boolean is_free { get; set; }

        public Volume(int volume_id, int book_id, bool is_free)
        {
            this.volume_id = volume_id;
            this.book_id = book_id;
            this.is_free = is_free;
        }

        public Volume()
        {
        }
    }
}
