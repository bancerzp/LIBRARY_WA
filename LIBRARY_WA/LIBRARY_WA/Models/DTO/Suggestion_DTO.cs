using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LIBRARY_WA.Models
{
    public class Suggestion_DTO
    {
       
        public int id { get; set; }
        public String title { get; set; }
        public String author_fullname { get; set; }
    }
}
