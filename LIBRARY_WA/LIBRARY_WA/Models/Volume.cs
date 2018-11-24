using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Volume
    {
        private object p;
        private int v1;
        private bool v2;

        public Volume(object p, int v1, bool v2)
        {
            this.p = p;
            this.v1 = v1;
            this.v2 = v2;
        }

        [Key]
        public int volume_id { get; set; }
        public int book_id { get; set; }
        public Boolean is_free { get; set; }
    }
}
