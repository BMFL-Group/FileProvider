using Data.Entities;
using Microsoft.AspNetCore.Http;

namespace FileProvider.Interfaces
{
    public interface IFileService
    {
        Task SaveToDatabaseAsync(FileEntity fileEntity);
        Task SetBlobContainerAsync(string containerName);
        string SetFileName(IFormFile file);
        Task<string> UploadFileAsync(IFormFile file, FileEntity fileEntity);
    }
}