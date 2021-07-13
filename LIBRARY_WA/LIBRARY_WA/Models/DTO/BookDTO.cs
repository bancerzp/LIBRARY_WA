using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class BookDTO
    {
        public int BookId { get; set; }

        [MaxLength(50, ErrorMessage = "Za długi tytuł")]
        public string Title { get; set; }

        [RegularExpression(@"\d{13}", ErrorMessage = "Niepoprawny format numer ISBN")]
        [StringLength(13)]
        public string Isbn { get; set; }

        [MaxLength(100, ErrorMessage = "Za długie nazwisko")]
        public string AuthorFullname { get; set; }

        [RegularExpression(@"\d{4}", ErrorMessage = "Niepoprawny format roku wydania książki")]
        public string Year { get; set; }

        [MaxLength(20, ErrorMessage = "Za długi język")]
        public string Language { get; set; }

        [MaxLength(30, ErrorMessage = "Za długi typ")]
        public string Type { get; set; }

        [MaxLength(300)]
        [System.ComponentModel.DefaultValue("Brak opisu")]
        public string Description { get; set; }


        //czy istnieje w bibliotece
        [System.ComponentModel.DefaultValue(true)]
        public bool IsAvailable { get; set; }

        public BookDTO(int bookId, string title, string isbn, string authorFullname, string year, string language, string type, string description, bool isAvailable)
        {
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            AuthorFullname = authorFullname;
            Year = year;
            Language = language;
            Type = type;
            Description = description;
            IsAvailable = isAvailable;
        }
    }
}