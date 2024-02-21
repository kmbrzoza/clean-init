using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Insig.Infrastructure.FileProcessing.Containers;
using Insig.Infrastructure.FileProcessing.Models;
using Insig.Infrastructure.FileProcessing.Services;
using Microsoft.Extensions.Configuration;

namespace Insig.Infrastructure.Services.FileServices;

public abstract class BaseFileService
{
    protected readonly IBlobService _blobService;
    protected readonly IConfiguration _configuration;

    private readonly long _maxImageSize;
    private readonly long _maxFileSize;

    private readonly string _storageApiUrl;

    protected virtual IReadOnlyList<string> AvailableFormats => new List<string>() { ".jfif", ".pjpeg", ".jpeg", ".pjp", ".jpg", ".png" }.AsReadOnly();

    public BaseFileService(IBlobService blobService, IConfiguration configuration)
    {
        _blobService = blobService;
        _configuration = configuration;

        _maxImageSize = long.Parse(_configuration["AppConfig:Validation:Images:MaxSizeInBytes"]);
        _maxFileSize = long.Parse(_configuration["AppConfig:Validation:Files:MaxSizeInBytes"]);

        _storageApiUrl = _configuration["AppConfig:StorageApiUrl"];
    }

    #region Store, Delete

    protected async Task Store<TContainer>(ImageFile blob) where TContainer : StorageContainer, new()
    {
        EnsureCorrectImageSize(blob);
        await _blobService.Store(new TContainer(), blob);
    }

    protected async Task Store<TContainer>(BlobFile blob) where TContainer : StorageContainer, new()
    {
        EnsureCorrectFileSize(blob);
        await _blobService.Store(new TContainer(), blob);
    }

    protected async Task Store<TContainer>(StreamBlobFile blob) where TContainer : StorageContainer, new()
    {
        EnsureCorrectFileSize(blob);
        await _blobService.Store(new TContainer(), blob);
    }

    protected async Task Delete<TContainer>(string fileName) where TContainer : StorageContainer, new()
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            await _blobService.Remove(new TContainer(), fileName);
        }
    }

    #endregion Store, Delete

    #region Validation

    protected void EnsureCorrectFormat(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        var correctFormat = AvailableFormats.Contains(extension.ToLower());

        if (!correctFormat)
        {
            throw new ArgumentException("File format not allowed.");
        }
    }

    private void EnsureCorrectImageSize(IBlobFile blob)
    {
        var isImageSizeExceeded = blob.FileContent.Length > _maxImageSize;
        if (isImageSizeExceeded)
        {
            throw new ArgumentException($"Image size is too large, maximum image size is {_maxImageSize / 1024 / 1024}MB");
        }
    }

    private void EnsureCorrectFileSize(IBlobFile blob)
    {
        var isFileSizeExceeded = blob.FileContent.Length > _maxFileSize;
        if (isFileSizeExceeded)
        {
            throw new ArgumentException($"File size is too large, maximum file size is {_maxFileSize / 1024 / 1024}MB");
        }
    }

    private void EnsureCorrectFileSize(IStreamBlobFile blob)
    {
        var isFileSizeExceeded = blob.Length > _maxFileSize;
        if (isFileSizeExceeded)
        {
            throw new ArgumentException($"File size is too large, maximum file size is {_maxFileSize / 1024 / 1024}MB");
        }
    }

    #endregion Validation

    #region BlobUrl, AuthQueryParams

    protected async Task<string> GetAuthBlobUrl<TContainer>(string blobName) where TContainer : StorageContainer, new()
    {
        return (await GetAuthBlobUrls<TContainer>(new List<string>() { blobName })).First();
    }

    protected async Task<List<string>> GetAuthBlobUrls<TContainer>(List<string> blobNames) where TContainer : StorageContainer, new()
    {
        var blobPaths = (await GetBlobPathsWithAuthQueryParams<TContainer>(blobNames)).ToList();

        return AddStorageApiUrlToBlobPaths(blobPaths);
    }

    protected string GetBlobUrl<TContainer>(string blobName) where TContainer : StorageContainer, new()
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return string.Empty;
        }

        return GetBlobUrls<TContainer>(new List<string>() { blobName }).First();
    }

    protected List<string> GetBlobUrls<TContainer>(List<string> blobNames) where TContainer : StorageContainer, new()
    {
        var blobPaths = GetBlobPaths<TContainer>(blobNames).ToList();

        return AddStorageApiUrlToBlobPaths(blobPaths);
    }

    private async Task<IList<string>> GetBlobPathsWithAuthQueryParams<TContainer>(IEnumerable<string> fileNames) where TContainer : StorageContainer, new()
    {
        if (!fileNames.Any())
        {
            return new List<string>();
        }

        return await _blobService.GenerateBlobPathsWithAuthQueryParams(new TContainer(), fileNames, BlobPermissions.Read);
    }

    private IList<string> GetBlobPaths<TContainer>(IEnumerable<string> fileNames) where TContainer : StorageContainer, new()
    {
        if (!fileNames.Any())
        {
            return new List<string>();
        }

        return _blobService.GenerateBlobPaths(new TContainer(), fileNames);
    }

    private List<string> AddStorageApiUrlToBlobPaths(List<string> blobPaths)
    {
        return blobPaths.Select(path => $"{_storageApiUrl}/{path}").ToList();
    }

    #endregion BlobUrl, AuthQueryParams
}