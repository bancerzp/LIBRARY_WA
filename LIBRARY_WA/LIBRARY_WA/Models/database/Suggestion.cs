using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models
{
    public class Suggestion
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorFullname { get; set; }

        public Suggestion(int id, string title, string authorFullname)
        {
            Id = id;
            Title = title;
            AuthorFullname = authorFullname;
        }
    }
}