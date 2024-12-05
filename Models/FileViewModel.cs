using System.Runtime.ExceptionServices;

namespace BlobStorage.Models
{
    public class FileViewModel
    {

        public File Current { get; set; } = null!;      

        public List<File> Children { get; set; }= new List<File>();


        public FileViewModel(File file)
        {
            Current = file;
        }


        public void AddItem(File item)
        {
            if(Current.Version < item.Version)
            {
                Children.Add(item);               
            }
            else
            {
                Children.Add(Current);
                Current = item;               
            }
            Children = Children.OrderBy(x => x.Version).ToList();
        }

    
    }
}
