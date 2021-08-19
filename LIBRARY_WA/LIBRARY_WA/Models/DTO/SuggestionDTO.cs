namespace Library.Models
{
    public class SuggestionDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorFullname { get; set; }

        public SuggestionDTO(int id, string title, string authorFullname)
        {
            Id = id;
            Title = title;
            AuthorFullname = authorFullname;
        }
    }
}