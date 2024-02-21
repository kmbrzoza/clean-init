using System.Collections.Generic;
using System.Threading.Tasks;
using Insig.Infrastructure.FileProcessing.Containers;
using Insig.Infrastructure.FileProcessing.Models;

namespace Insig.Infrastructure.FileProcessing.Services;

public interface IBlobService
{
    Task Store(StorageContainer storageContainer, IBlobFile file);
    Task Store(StorageContainer storageContainer, IStreamBlobFile file);
    Task Remove(StorageContainer storageContainer, string fileName);
    Task<string> GenerateBlobPathWithAuthQueryParams(StorageContainer storageContainer, string blobName, BlobPermissions permissions, uint expirationMinutes = 30);
    Task<IList<string>> GenerateBlobPathsWithAuthQueryParams(StorageContainer storageContainer, IEnumerable<string> blobNames, BlobPermissions permissions, uint expirationMinutes = 30);
    string GenerateBlobPath(StorageContainer storageContainer, string blobName);
    IList<string> GenerateBlobPaths(StorageContainer storageContainer, IEnumerable<string> blobNames);
}
