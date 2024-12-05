using BlobStorage.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Xml.Linq;
using File = BlobStorage.Models.File;

namespace BlobStorage.Services
{
    public class DataBaseService
    {
        private readonly ApplicationContext db;

        public DataBaseService(ApplicationContext appContext)
        {
            this.db = appContext;
        }


       
        public async Task<List<File>> GetFilesAsync(Expression<Func<File, bool>>? filter = null)                                 
            
        {
            List<File> files=new List<File>();
            if (filter != null)
               files = await db.Files.Where(filter).ToListAsync();
            else
                files= await db.Files.ToListAsync();                   

            return files;
        }



        public async Task<File?> GetFileByIdAsync(int Id)
        {
            File? file= await db.Files.FirstOrDefaultAsync(f => f.Id == Id);
            return file;
        }



        public async Task<int> GetCountAsync(Expression<Func <File,bool>>? filter=null)
        {
            if (filter != null)
               return await  db.Files.Where(filter).CountAsync();
            return await db.Files.CountAsync();
        }

    

        public async Task<bool> DeleteFileAsync(int id)
        {
            File? currentFile = await db.Files.FirstOrDefaultAsync(x=>x.Id==id);
            if (currentFile == null)
                return false;
            List<File> files = await GetFilesAsync(x => x.Name == currentFile.Name && x.Id!=currentFile.Id);
            files=files.OrderByDescending(x=>x.CreateAt).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                files[i].Version = (byte)(i+1);
            }
            db.Files.Remove(currentFile);
            await db.SaveChangesAsync();
            return true;    
       
        }

        public  async Task<File> AddFileAsync(File file)
        {
            List<File> files = await GetFilesAsync(x=>x.Name==file.Name);

           foreach(File f in files)
            {
                f.Version = (byte)((f.Version ?? 0) + 1);
            }
            await db.Files.AddAsync(file);
            await db.SaveChangesAsync();
            return file;
        }

        public  async Task<bool> DoMainFile(int id, string fileName)
        {     
            List<File> files= await GetFilesAsync(x=>x.Name == fileName);
            if(files.FirstOrDefault(x=>x.Id==id)==null)
                return false;          
                     
             files=files.OrderByDescending(x=>x.CreateAt).ToList();
            bool flag = false;
             for(byte i=0;i<files.Count;i++)
             {
                if (files[i].Id == id)
                {
                    flag= true;
                    files[i].Version = 1;
                }
                else
                {
                    files[i].Version = flag? (byte)(i+1): (byte)(i+2);
                }
             }
            await db.SaveChangesAsync();
            return true;
        }
       

    }
}
