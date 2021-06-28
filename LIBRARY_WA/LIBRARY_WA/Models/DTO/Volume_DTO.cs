namespace LIBRARY_WA.Models
{
    public class Volume_DTO
    {
        public int VolumeId { get; set; }
        public int BookId { get; set; }
        public bool IsFree { get; set; }

        public Volume_DTO()
        {
        }

        public Volume_DTO(int bookId, bool isFree)
        {
            BookId = bookId;
            IsFree = isFree;
        }

        public Volume_DTO(int volumeId, int bookId, bool isFree)
        {
            VolumeId = volumeId;
            BookId = bookId;
            IsFree = isFree;
        }
    }
}