using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_WA.Models
{
    public class Book
    {
        public Book(int book_id,string title, string isbn, int author_id, string year, string language, string type, string description, bool is_available)
        {
            this.book_id = book_id;
            this.title = title;
            this.isbn = isbn;
            this.author_id = author_id;
            this.year = year;
            this.language = language;
            this.type = type;
            this.description = description;
            this.is_available = is_available;
        }

        public Book(string title, string isbn, int author_id, string year, string language, string type, string description, bool is_available)
        {
           
            this.title = title;
            this.isbn = isbn;
            this.author_id = author_id;
            this.year = year;
            this.language = language;
            this.type = type;
            this.description = description;
            this.is_available = is_available;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int book_id { get; set; }

        [MaxLength(50, ErrorMessage = "Za długi tytuł")]
        public string title { get; set; }

        [RegularExpression(@"\d{13}", ErrorMessage = "Niepoprawny format numer ISBN")]
        [StringLength(13)]
        public string isbn { get; set; }

        [MaxLength(100, ErrorMessage = "Za długie nazwisko")]
        public int author_id { get; set; }

        [RegularExpression(@"\d{4}", ErrorMessage = "Niepoprawny format roku wydania książki")]
        public string year { get; set; }

        [MaxLength(20, ErrorMessage = "Za długi język")]
        public string language { get; set; }

        [MaxLength(30, ErrorMessage = "Za długi typ")]
        public string type { get; set; }

        [MaxLength(300)]
        [System.ComponentModel.DefaultValue("Brak opisu")]
        public string description { get; set; }


        //czy istnieje w bibliotece
        [System.ComponentModel.DefaultValue(true)]
        public bool is_available { get; set; }
    }
}