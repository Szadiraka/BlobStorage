namespace BlobStorage.Models
{
    public class User
    {
        public int Id {  get; set; }
        public String? Name { get; set; }
        public String? Surname { get; set; }
        public String? Email { get; set; }
        public String? Phone { get; set; }

        public List<File> Files { get; set; }= new List<File>();

    }
}
