using System;

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
        public string title { get; set; }
        public string author_fullname { get; set; }
    }
}