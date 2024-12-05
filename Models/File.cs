namespace BlobStorage.Models
{
    public class File
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreateAt { get; set; }
        public double? Size { get;set; }
        public byte? Version { get; set; }   
        public string? PathToBlobItem { get; set; }  

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
