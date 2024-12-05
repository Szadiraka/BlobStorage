namespace BlobStorage.Models
{
    public class PaginationItem
    {
        public bool IsBefore { get; set; } =false;

        public bool IsAfter { get; set; } = false;

        public int CurrentPage { get; set;} = 1;
    }
}
