using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Suggestion_DTO
    {
        public Suggestion_DTO(int id, string title, string author_fullname)
        {
            this.id = id;
            this.title = title;
            this.author_fullname = author_fullname;
        }

        public int id { get; set; }
        public String title { get; set; }
        public String author_fullname { get; set; }
    }
}
