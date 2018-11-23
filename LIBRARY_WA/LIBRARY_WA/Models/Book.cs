using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Book
    {
        [Key]
        public Int32 book_id { get; set; }

        [MaxLength(50)]
        public string title { get; set; }

        [RegularExpression(@"\d{13}", ErrorMessage = "Niepoprawny format numer ISBN")]
        [StringLength(13)]
        public string ISBN { get; set; }
        
        [MaxLength(100)]
        public string author_fullname { get; set; }

        [RegularExpression(@"\d{4}", ErrorMessage = "Niepoprawny format roku wydania książki")]
        public String year { get; set; }

        [MaxLength(20)]
        public string language { get; set; }
        [MaxLength(30)]
        public string type { get; set; }

        [MaxLength(300)]
        public string description { get; set; }

        public Boolean is_available { get; set; }
    }
}
