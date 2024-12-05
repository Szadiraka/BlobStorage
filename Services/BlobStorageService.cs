using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlobStorage.Services
{

   
    public class BlobStorageService: IBlobStorageService
    {

        private readonly BlobServiceClient blobServiceClient;
        private readonly string? containerName;
        private readonly BlobContainerClient ContainerClient;

        public BlobStorageService(BlobServiceClient client, IConfiguration config) 
        {
            this.blobServiceClient = client;
            this.containerName = config.GetSection("BlobStorageSettings").GetValue<string>("ContainerName")??"";
            ContainerClient=blobServiceClient.GetBlobContainerClient(containerName);
        }


        public async Task<string?> UploadFileAsync(string blobName, Stream content)
        {
            try{
                await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                var blobClient = ContainerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(content, overwrite: true);
                return blobClient?.Uri.ToString();
            }catch (Exception )
            {
                return null;
            }
           
            
        }


        public async Task<Stream?> DownloadFileAsync(string blobName)
        {
           
            var blobClient = ContainerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
               return await blobClient.OpenReadAsync();
            }

            return null;
        }

        public async Task<bool> DeleteFileAsync(string blobName)
        {
          
            var blobClient = ContainerClient.GetBlobClient(blobName);

            bool result =await blobClient.DeleteIfExistsAsync();
            return result;
        }

    }
}
