using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class Book
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [MaxLength(50, ErrorMessage = "Za długi tytuł")]
        public string Title { get; set; }

        [RegularExpression(@"\d{13}", ErrorMessage = "Niepoprawny format numer ISBN")]
        [StringLength(13)]
        public string Isbn { get; set; }

        [MaxLength(100, ErrorMessage = "Za długie nazwisko")]
        public int AuthorId { get; set; }

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

        public Book(int bookId, string title, string isbn, int author_id, string year, string language, string type, string description, bool isAvailable)
        {
            BookId = bookId;
            Title = title;
            Isbn = isbn;
            AuthorId = author_id;
            Year = year;
            Language = language;
            Type = type;
            Description = description;
            IsAvailable = isAvailable;
        }

        public Book(string title, string isbn, int author_id, string year, string language, string type, string description, bool isAvailable)
        {
            Title = title;
            Isbn = isbn;
            AuthorId = author_id;
            Year = year;
            Language = language;
            Type = type;
            Description = description;
            IsAvailable = isAvailable;
        }

        public Book()
        {

        }
    }
}