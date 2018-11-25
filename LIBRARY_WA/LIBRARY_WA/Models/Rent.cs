using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models
{
    public class Rent
    {
        [Key]
        public Int32 RENT_ID { get; set; }

        public string USER_ID { get; set; }

        public Int32 BOOK_ID { get; set; }
        public String TITLE { get; set; }
        public String ISBN { get; set; }

        public Int32 VOLUME_ID { get; set; }

        public DateTime START_DATE { get; set; }

        public DateTime EXPIRE_DATE { get; set; }

    }
}
