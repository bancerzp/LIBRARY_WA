using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Reservation
    {
        [Key]
        public Int32 RESERVATION_ID { get; set; }
        public Int32 USER_ID { get; set; }
        public String TITLE { get; set; }
        public String ISBN { get; set; }
        public Int32 BOOK_ID { get; set; }
        public Int32 VOLUME_ID { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public Boolean IS_ACTIVE { get; set; }
    }
}
