using BlobStorage.Models;
using File = BlobStorage.Models.File;
using BlobStorage.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;



namespace BlobStorage.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlobStorageService blobStorageService;
       
        private readonly DataBaseService dataBaseService;
        private int countOfFilesOnPage;
        private int number = 1;

        public HomeController(IConfiguration config, IBlobStorageService service, DataBaseService db)
        {           
            blobStorageService = service;
            this.dataBaseService = db;
            int.TryParse(config.GetSection("CountElements").GetValue<string>("numbers") ?? "",out countOfFilesOnPage);
        }

        public async Task<IActionResult> Index(MainModel mainModel)
        {

            if (TempData["message"] != null && TempData["signal"] != null) {
                ViewBag.Message = TempData["message"];
                ViewBag.Signal = TempData["signal"];
            }
          
            var files = await GetFileByFilter(mainModel);

            List<FileViewModel> filesViewModels = CreateListFileViewModel(files); 
            number=filesViewModels.Count;
           
            filesViewModels = SortFilesViewModelsList(filesViewModels,mainModel.CurrentSort, countOfFilesOnPage,mainModel.PaginationItem.CurrentPage);

            MainModel model = CreateMainModel(filesViewModels, mainModel);          

            return View(model);

        }

        private MainModel CreateMainModel(List<FileViewModel> fileViewModels, MainModel mainModel)
        {           
            var filtrationItem = new FiltrationItem() {
                FileName = mainModel.FiltrationItem.FileName ?? "",
                From = mainModel.FiltrationItem.From ,
                To = mainModel.FiltrationItem.To 
            };


            var paginationItem = new PaginationItem()
            {
                CurrentPage = mainModel.PaginationItem.CurrentPage,
                IsBefore = mainModel.PaginationItem.CurrentPage>1,
                IsAfter = number - (countOfFilesOnPage* mainModel.PaginationItem.CurrentPage) > 0 ,
            };

             MainModel model = new MainModel(filtrationItem, paginationItem, fileViewModels);
              model.CurrentSort = mainModel.CurrentSort;
              model.CountOfFilesOnPage = countOfFilesOnPage;
            return model;
        }

        private List<FileViewModel> SortFilesViewModelsList(List<FileViewModel> filesViewModels,SortedFile marker, int count, int currentPage)
        {
            List < FileViewModel> newList=new();
            switch (marker)
            {
                case SortedFile.ByName:
                    newList= filesViewModels.OrderBy(x => x.Current.Name).Skip(count*(currentPage-1)).Take(count).ToList();
                    break;
                case SortedFile.ByNameDesc:
                    newList = filesViewModels.OrderByDescending(x => x.Current.Name).Skip(count * (currentPage - 1)).Take(count).ToList();
                    break;
                case SortedFile.ByDate:
                    newList = filesViewModels.OrderBy(x => x.Current.CreateAt).Skip(count * (currentPage - 1)).Take(count).ToList();
                    break;
                case SortedFile.ByDateDesc:
                    newList = filesViewModels.OrderByDescending(x => x.Current.CreateAt).Skip(count * (currentPage - 1)).Take(count).ToList();
                    break;
                default:
                    return filesViewModels;
               
            }
            return newList;
        }


        private async Task<List<File>> GetFileByFilter(MainModel mainModel)
        {
            var from = mainModel.FiltrationItem.From;
            var to = mainModel.FiltrationItem.To;
            var name = mainModel.FiltrationItem.FileName;
            List<File> files = new ();

            if (string.IsNullOrEmpty(name))
            {
              
                files = await dataBaseService.GetFilesAsync(x=>x.CreateAt>=from && x.CreateAt<=to);  
            }
            else
            {
               files =await  dataBaseService.GetFilesAsync(x => x.CreateAt >= from && x.CreateAt <= to && x.Name!.ToLower().Contains(name.ToLower()));
            }

            return files;
           
        }


         private List<FileViewModel> CreateListFileViewModel(List<File> files)
        {
            List<FileViewModel> filesViewModels = new List<FileViewModel>();
          
                foreach (var file in files)
                {
                    FileViewModel? item = filesViewModels.FirstOrDefault(x => x.Current.Name == file.Name);
                    if (item != null)
                    {
                        item.AddItem(file);
                    }
                    else
                    {
                        filesViewModels.Add(new FileViewModel(file));
                    }
                }
      
           
            return filesViewModels;
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task< IActionResult> UpLoadFile(IFormFile file)
        {
            if(file!=null && file.Length>0)
            {               
                string fileName=file.FileName;
                string blobName=fileName+":"+Guid.NewGuid().ToString();
                using(var stream= file.OpenReadStream())
                {
                    string? result = await blobStorageService.UploadFileAsync(blobName, stream);
                    if (result!=null)
                    {
                        var res = Math.Round(file.Length / 1024.0, 1);
                        File newFile = new()
                        {
                            Name = fileName,
                            CreateAt = DateTime.Now,
                            Version = 1,
                            PathToBlobItem = blobName,
                            UserId = 1,
                            Size =res 
                        }; 
                        var f =await dataBaseService.AddFileAsync(newFile);
                        if (f == null)
                        {
                            TempData["message"] = "Файл не удалось сохранить в бд";
                            TempData["signal"] = "Bad";
                        }
                            
                        else
                        {
                            TempData["message"] = "Файл сохранен";
                            TempData["signal"] = "Ok";
                        }
                           
                    }

                    else
                    {
                        TempData["message"] = "Файл не удалось загрузить в blob-хранилище";
                        TempData["signal"] = "Bad";
                    }

                }
              

            }
            else
            {
                TempData["message"] = "Файл не получен от пользователя";
                TempData["signal"] = "Bad";
            }
        
             
            return RedirectToAction("Index");
        }


        [HttpPost("Home/Delete")]
        public async Task<IActionResult> DeleteFile(int id, string name)
        {
            File? currentFile = await dataBaseService.GetFileByIdAsync(id);
            if (currentFile == null)
            {
                TempData["message"] = "Файл не найден";
                TempData["signal"] = "Bad";
                return RedirectToAction("Index");
            }
            bool result = await dataBaseService.DeleteFileAsync(id);
         
            bool result2 = await blobStorageService.DeleteFileAsync(currentFile.PathToBlobItem!);
            if (!result2)
            {               
                    TempData["message"] = "Файл удален из blob-хранилища но не удален из бд";
                    TempData["signal"] = "Bad";

            }
            else
            {
                TempData["message"] = "Все удалено!";
                TempData["signal"] = "OK";
            }
           
          
            return RedirectToAction("Index");

        }

        

        [HttpPost("Home/DownLoad")]
        public async Task<IActionResult> DownLoadFile(int id)
        {            
            File? currentFile = await dataBaseService.GetFileByIdAsync(id);
            if (currentFile == null)
            {
                TempData["message"] = "Файл не найден";
                TempData["signal"] = "Bad";
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    Stream? stream = await blobStorageService.DownloadFileAsync(currentFile.PathToBlobItem!);
                    {
                        if (stream == null)
                        {
                            TempData["message"] = "Файл не найден в блобе";
                            TempData["signal"] = "Bad";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            if (!stream.CanRead)
                            {
                                TempData["message"] = "Файл поврежден или пуст";
                                TempData["signal"] = "Bad";
                                return RedirectToAction("Index");
                            }

                            return File(stream, "application/octet-stream", currentFile.Name);
                        }
                    }
                }catch (Exception ex)
                {
                    TempData["message"] = "Ошибка при загрузке файла:";
                    TempData["signal"] = "Bad";
                    return RedirectToAction("Index");
                }
               

            }

            
        }


        [HttpPost("Home/MakePrimary")]
        public async Task<IActionResult> MakeFilePrimary(int id, string name)
        {
        
            if (await dataBaseService.DoMainFile(id,name))
            {
                TempData["message"] = "Корректировка выполнена";
                TempData["signal"] = "Ok";
               
            }
            else
            {
                TempData["message"] = "Не удалось выполнить изменения";
                TempData["signal"] = "Bad";
            }
            return RedirectToAction("Index");
        }
    }
}
