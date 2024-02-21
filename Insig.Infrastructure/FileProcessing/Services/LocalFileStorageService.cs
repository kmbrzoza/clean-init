using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Insig.Common.FileProcessing;
using Insig.Infrastructure.FileProcessing.Containers;
using Insig.Infrastructure.FileProcessing.Models;

namespace Insig.Infrastructure.FileProcessing.Services;

public class LocalFileStorageService : IBlobService
{
    public async Task Store(StorageContainer storageContainer, IBlobFile file)
    {
        var directoryPath = file.HasParentDirectory() ?
            Path.Combine(GetResourcesPath(storageContainer), file.ParentDirectory) :
            GetResourcesPath(storageContainer);

        Directory.CreateDirectory(directoryPath);

        await using (var stream = new FileStream(Path.Combine(directoryPath, file.FileName), FileMode.Create))
        {
            await stream.WriteAsync(file.FileContent);
        }
    }

    public async Task Store(StorageContainer storageContainer, IStreamBlobFile file)
    {
        var directoryPath = file.HasParentDirectory() ?
           Path.Combine(GetResourcesPath(storageContainer), file.ParentDirectory) :
           GetResourcesPath(storageContainer);

        Directory.CreateDirectory(directoryPath);

        await using (var stream = new FileStream(Path.Combine(directoryPath, file.FileName), FileMode.Create))
        {
            await file.FileStream.CopyToAsync(stream);
        }
    }

    public async Task Remove(StorageContainer storageContainer, string filePath)
    {
        var directoryPath = GetResourcesPath(storageContainer);
        var directoryName = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directoryName))
        {
            directoryPath = Path.Combine(directoryPath, directoryName);
        }

        var fileName = Path.GetFileName(filePath);
        var filePaths = Directory.GetFiles(directoryPath);
        var fileToRemove = filePaths.FirstOrDefault(x => x.Contains(fileName));

        if (fileToRemove != null)
        {
            await DeleteAsync(fileToRemove);
        }
    }

    public async Task<string> GenerateBlobPathWithAuthQueryParams(StorageContainer storageContainer, string blobName, BlobPermissions permissions, uint expirationMinutes = 30)
    {
        return await Task.FromResult($"{storageContainer.Name}/{blobName}");
    }

    public async Task<IList<string>> GenerateBlobPathsWithAuthQueryParams(StorageContainer storageContainer, IEnumerable<string> blobNames, BlobPermissions permissions, uint expirationMinutes = 30)
    {
        return await Task.FromResult(blobNames.Select(b => $"{storageContainer.Name}/{b}").ToList());
    }

    public IList<string> GenerateBlobPaths(StorageContainer storageContainer, IEnumerable<string> blobNames)
    {
        return blobNames.Select(b => $"{storageContainer.Name}/{b}").ToList();
    }

    public string GenerateBlobPath(StorageContainer storageContainer, string blobName)
    {
        return $"{storageContainer.Name}/{blobName}";
    }

    private Task DeleteAsync(string filePath)
    {
        return Task.Factory.StartNew(() => File.Delete(filePath));
    }

    private string GetResourcesPath(StorageContainer storageContainer)
    {
        EnsureArg.IsNotNull(storageContainer, nameof(storageContainer));

        var apiPath = Projects.GetProjectPath(ProjectEnum.Api);

        return Path.Combine(apiPath, "Resources", storageContainer.Name);
    }
}
