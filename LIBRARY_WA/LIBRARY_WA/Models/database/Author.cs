using System;
using System.ComponentModel.DataAnnotations;

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