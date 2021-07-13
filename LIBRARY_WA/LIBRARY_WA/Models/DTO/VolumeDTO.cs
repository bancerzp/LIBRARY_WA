namespace Library.Models
{
    public class VolumeDTO
    {
        public int VolumeId { get; set; }
        public int BookId { get; set; }
        public bool IsFree { get; set; }

        public VolumeDTO()
        {
        }

        public VolumeDTO(int bookId, bool isFree)
        {
            BookId = bookId;
            IsFree = isFree;
        }

        public VolumeDTO(int volumeId, int bookId, bool isFree)
        {
            VolumeId = volumeId;
            BookId = bookId;
            IsFree = isFree;
        }
    }
}