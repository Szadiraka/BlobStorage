namespace BlobStorage.Models
{
    public class MainModel
    {
        public List<SortedFile> SortedBy { get;} =new List<SortedFile>() {SortedFile.ByName,SortedFile.ByNameDesc,
                                                                  SortedFile.ByDate,SortedFile.ByDateDesc};

        public SortedFile CurrentSort { get; set; } = SortedFile.ByDateDesc;
        public int CountOfFilesOnPage { get; set; }    

        public FiltrationItem FiltrationItem { get; set;}
        public PaginationItem PaginationItem { get; set; }
        public  List<FileViewModel> Models { get; set; }=new List<FileViewModel>();

        public MainModel(FiltrationItem filtrationItem, PaginationItem paginationItem, List<FileViewModel> files)
        {
            this.FiltrationItem = filtrationItem;
            this.PaginationItem = paginationItem;
            this.Models = files;
        }

        public MainModel()
        {
            FiltrationItem = new();
            PaginationItem = new();
        }
    }
}
