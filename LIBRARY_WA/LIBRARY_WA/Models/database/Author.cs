using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models.database
{
    public class Author
    {
        public Author(string author_fullname)
        {
            this.author_fullname = author_fullname;
        }

        [Key]
        public int author_id { get; set; }
        public String author_fullname { get; set; }

    }
}
