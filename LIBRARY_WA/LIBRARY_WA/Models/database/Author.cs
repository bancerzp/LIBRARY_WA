using System.ComponentModel.DataAnnotations;

namespace LIBRARY_WA.Models.database
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string AuthorFullname { get; set; }

        public Author(string authorFullname)
        {
            AuthorFullname = authorFullname;
        }
    }
}