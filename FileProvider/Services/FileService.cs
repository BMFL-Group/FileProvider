using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Data.Contexts;
using Data.Entities;
using FileProvider.Functions;
using FileProvider.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FileProvider.Services;

public class FileService(ILogger<FileService> logger, DataContext context, BlobServiceClient client) 
{
    private readonly ILogger<FileService> _logger = logger;
    private readonly DataContext _context = context;
    private readonly BlobServiceClient _client = client; //koppling till serivce bus
    private BlobContainerClient? _container;

    public async Task SetBlobContainerAsync(string containerName)
    {
        _container = _client.GetBlobContainerClient(containerName); //hämtar in en container, vad för typ av container och inte private, står i dokumentationen 
        await _container.CreateIfNotExistsAsync(PublicAccessType.BlobContainer); //skapar om den inte finns
    }

    public string SetFileName(IFormFile file)
    {
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        return fileName;
    }

    public async Task<string> UploadFileAsync(IFormFile file, FileEntity fileEntity)
    {
        // Laddar upp filen som man har 
        BlobHttpHeaders headers = new()
        {
            ContentType = file.ContentType,
        };

        var blobClient = _container!.GetBlobClient(fileEntity.FileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, headers);

        return blobClient.Uri.ToString(); // får tillbaka filnamnet 
    }

    public async Task SaveToDatabaseAsync(FileEntity fileEntity)
    {
        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();
    }

}
