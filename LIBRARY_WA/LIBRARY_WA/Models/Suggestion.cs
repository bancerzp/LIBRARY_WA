using System;
using System.ComponentModel.DataAnnotations;
namespace LIBRARY_WA.Models
{
    public class Suggestion
    {
        [Key]
        public int id { get; set; }
        public String title { get; set; }
        public String author_fullname { get; set; }
    }
}
