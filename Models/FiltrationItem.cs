namespace BlobStorage.Models
{
    public class FiltrationItem
    {
        public string? FileName {  get; set; }
        public DateTime From { get; set; } 
        public DateTime To { get; set;}


        public FiltrationItem()
        {
            From = DateTime.Parse("2020-01-01");
            To = DateTime.Now; 
        }

    }

    
}
