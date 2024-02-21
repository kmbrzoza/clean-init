using System;
using System.IO;
using EnsureThat;
using Insig.Infrastructure.FileProcessing.Helpers;

namespace Insig.Infrastructure.FileProcessing.Models;

public class ImageFile : BaseBlobFile, IBlobFile
{
    public string FileName { get; }
    public byte[] FileContent { get; }
    public string ContentType { get; }
    public string ParentDirectory { get; }
    public string FilePath { get; }

    public ImageFile(string imageAsBase64, string parentDirectory = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(imageAsBase64, nameof(imageAsBase64));

        var sanitizedImageAsBase64 = FileProcessingHelper.GetSanitizedBase64(imageAsBase64);
        var imageFormat = $".{FileProcessingHelper.GetSanitizedBase64FileType(imageAsBase64)}";

        FileName = GenerateFileName(imageFormat);
        FileContent = Convert.FromBase64String(sanitizedImageAsBase64);
        ContentType = GetMimeType(imageFormat);
        ParentDirectory = parentDirectory;
        FilePath = string.IsNullOrWhiteSpace(parentDirectory) ? FileName : Path.Combine(ParentDirectory, FileName);
    }

    public bool HasParentDirectory()
    {
        return !string.IsNullOrWhiteSpace(ParentDirectory);
    }

    private string GetMimeType(string imageFormat) =>
        imageFormat switch
        {
            ".png" => "image/png",
            _ => "image/jpeg"
        };
}