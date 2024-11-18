using Azure.Storage.Blobs;
using LmsApplication.ResourceModule.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace LmsApplication.ResourceModule.Services.BlobClients;

public interface IBlobClient
{
    Task<Stream> DownloadBlobAsync(ResourceMetadata metadata);
    
    Task UploadBlobAsync(ResourceMetadata metadata, IFormFile file);
    
    Task DeleteBlobAsync(ResourceMetadata metadata);
}

public class BlobClient : IBlobClient
{
    private readonly BlobContainerClient _blobContainerClient;

    public BlobClient(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }

    public async Task<Stream> DownloadBlobAsync(ResourceMetadata metadata)
    {
        var blobName = $"{metadata.Type}/{metadata.ParentId}/{metadata.Id}{metadata.FileExtension}";
        var blob = await _blobContainerClient.GetBlobClient(blobName).DownloadAsync();
        if (blob?.Value is null)
            throw new Exception("Failed to download blob.");
        
        return blob.Value.Content;
    }

    public async Task UploadBlobAsync(ResourceMetadata metadata, IFormFile file)
    {
        var blobName = $"{metadata.Type}/{metadata.ParentId}/{metadata.Id}{metadata.FileExtension}";
        var result = await _blobContainerClient.UploadBlobAsync(blobName, file.OpenReadStream());
        if (result?.Value is null)
            throw new Exception("Failed to upload blob.");
    }

    public async Task DeleteBlobAsync(ResourceMetadata metadata)
    {
        var blobName = $"{metadata.Type}/{metadata.ParentId}/{metadata.Id}{metadata.FileExtension}";
        var result = await _blobContainerClient.DeleteBlobAsync(blobName);
        if (result is null)
            throw new Exception("Failed to delete blob.");
    }
}