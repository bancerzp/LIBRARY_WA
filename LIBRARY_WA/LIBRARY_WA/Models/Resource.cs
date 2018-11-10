using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Resource
    {
        [Required]
        public int Id { get; set; }

        public string BookId { get; set; }

        [StringLength(100)]
        public string title { get; set; }

        [RegularExpression(@"\d{13}", ErrorMessage = "Niepoprawny format numer ISBN")]
        [StringLength(13)]
        public string ISBN { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(40)]
        public string authorFirstName { get; set; }

        [StringLength(40)]
        public string authorSurname { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy.MMM.dd}")]
        public DateTime releaseDate { get; set; }

        [RegularExpression(@"\d{4}", ErrorMessage = "Niepoprawny format roku wydania książki")]
        public String year { get; set; }

        public string language { get; set; }


    }
}
