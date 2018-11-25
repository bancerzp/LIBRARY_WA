using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Renth
    {

        [Key]
        public Int32 RENT_ID_H { get; set; }
        public Int32 USER_ID { get; set; }
        public String TITLE { get; set; }
        public String ISBN { get; set; }
        public Int32 BOOK_ID { get; set; }
        public Int32 VOLUME_ID { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }
}
