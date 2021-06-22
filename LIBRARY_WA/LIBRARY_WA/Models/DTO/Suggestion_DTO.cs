namespace LIBRARY_WA.Models
{
    public class Suggestion_DTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorFullname { get; set; }

        public Suggestion_DTO(int id, string title, string authorFullname)
        {
            Id = id;
            Title = title;
            AuthorFullname = authorFullname;
        }
    }
}