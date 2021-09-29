using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class Volume
    {
        [Key]
        public int VolumeId { get; set; }
        public int BookId { get; set; }
        public bool IsFree { get; set; }

        public Volume(int bookId, bool isFree)
        {
            BookId = bookId;
            IsFree = isFree;
        }

        public Volume()
        {
        }
    }
}