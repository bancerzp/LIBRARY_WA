using System;
using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models
{
    public class Suggestion
    {
        public Suggestion(int id, string title, string author_fullname)
        {
            this.id = id;
            this.title = title;
            this.author_fullname = author_fullname;
        }

        [Key]
        public int id { get; set; }
        public String title { get; set; }
        public String author_fullname { get; set; }
    }
}