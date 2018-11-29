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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rent_id_h { get; set; }
        public int user_id { get; set; }
        [MaxLength(50)]
        public String title { get; set; }

        [StringLength(13)]
        public String isbn { get; set; }
        public int book_id { get; set; }
        public int volume_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}
