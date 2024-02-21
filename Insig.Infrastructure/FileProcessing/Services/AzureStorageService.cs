using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using EnsureThat;
using Insig.Infrastructure.FileProcessing.Containers;
using Insig.Infrastructure.FileProcessing.Models;

namespace Insig.Infrastructure.FileProcessing.Services;

public class AzureStorageService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string BlobResourceType = "b";

    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task Store(StorageContainer storageContainer, IBlobFile file)
    {
        EnsureArg.IsNotNull(file, nameof(file));

        var containerClient = await GetContainerClient(storageContainer);
        await UploadToStorage(containerClient, file);
    }

    public async Task Store(StorageContainer storageContainer, IStreamBlobFile file)
    {
        EnsureArg.IsNotNull(file, nameof(file));

        var containerClient = await GetContainerClient(storageContainer);
        await UploadToStorage(containerClient, file);
    }

    public async Task Remove(StorageContainer storageContainer, string fileName)
    {
        EnsureArg.IsNotNull(storageContainer, nameof(storageContainer));
        EnsureArg.IsNotNullOrWhiteSpace(fileName, nameof(fileName));

        var containerClient = _blobServiceClient.GetBlobContainerClient(storageContainer.Name);
        await containerClient.GetBlobClient(fileName).DeleteIfExistsAsync();
    }

    public async Task<string> GenerateBlobPathWithAuthQueryParams(StorageContainer storageContainer, string blobName, BlobPermissions permissions, uint expirationMinutes = 30)
    {
        var result = await GenerateBlobPathsWithAuthQueryParams(storageContainer, new List<string>() { blobName }, permissions, expirationMinutes);
        return result.First();
    }

    public async Task<IList<string>> GenerateBlobPathsWithAuthQueryParams(StorageContainer storageContainer, IEnumerable<string> blobNames, BlobPermissions permissions, uint expirationMinutes = 30)
    {
        var blobContainerClient = await GetContainerClient(storageContainer);

        var result = new List<string>();

        foreach (var blobName in blobNames)
        {
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            if (blobClient.CanGenerateSasUri)
            {
                if (!await blobClient.ExistsAsync())
                {
                    throw new ArgumentException($"Blob {blobName} does not exist in {storageContainer.Name} container");
                }

                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = BlobResourceType,
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
                };

                sasBuilder.SetPermissions(GetBlobContainerSasPermissions(permissions));

                var sasUri = blobClient.GenerateSasUri(sasBuilder);

                result.Add($"{storageContainer.Name}/{blobName}{sasUri.Query}");
            }
            else
            {
                throw new Exception("Cannot generate SAS Uri due to lack of authentication with Azure Storage");
            }
        }

        return result;
    }

    public string GenerateBlobPath(StorageContainer storageContainer, string blobName)
    {
        return $"{storageContainer.Name}/{blobName}";
    }

    public IList<string> GenerateBlobPaths(StorageContainer storageContainer, IEnumerable<string> blobNames)
    {
        return blobNames.Select(b => $"{storageContainer.Name}/{b}").ToList();
    }

    private async Task<BlobContainerClient> GetContainerClient(StorageContainer storageContainer)
    {
        EnsureArg.IsNotNull(storageContainer, nameof(storageContainer));

        var containerClient = _blobServiceClient.GetBlobContainerClient(storageContainer.Name);

        if (!(await containerClient.ExistsAsync()))
        {
            var accessType = storageContainer.IsPrivate ? PublicAccessType.None : PublicAccessType.Blob;
            await containerClient.CreateAsync(accessType);
        }

        return containerClient;
    }

    private async Task UploadToStorage(BlobContainerClient containerClient, IBlobFile file)
    {
        var blobClient = containerClient.GetBlobClient(file.FilePath);

        await using (var stream = new MemoryStream(file.FileContent))
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
        }
    }

    private async Task UploadToStorage(BlobContainerClient containerClient, IStreamBlobFile file)
    {
        var blobClient = containerClient.GetBlobClient(file.FilePath);

        await blobClient.UploadAsync(file.FileStream, new BlobHttpHeaders { ContentType = file.ContentType });
    }

    private BlobContainerSasPermissions GetBlobContainerSasPermissions(BlobPermissions permissions)
    {
        return permissions switch
        {
            BlobPermissions.Read => BlobContainerSasPermissions.Read,
            _ => throw new ArgumentException("Unsupported value")
        };
    }
}
