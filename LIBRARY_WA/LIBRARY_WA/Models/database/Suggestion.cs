using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class Suggestion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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