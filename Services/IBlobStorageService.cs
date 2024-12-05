namespace BlobStorage.Services
{
    public interface IBlobStorageService
    {
        Task<string?> UploadFileAsync(string blobName, Stream content);

        Task<Stream?> DownloadFileAsync(string blobName);

        Task<bool> DeleteFileAsync(string blobName);
    }

}
