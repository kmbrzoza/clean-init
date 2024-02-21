using System.IO;
using EnsureThat;

namespace Insig.Infrastructure.FileProcessing.Models;

public class BlobFile : BaseBlobFile, IBlobFile
{
    public string FileName { get; }
    public byte[] FileContent { get; }
    public string ContentType { get; }
    public string ParentDirectory { get; }
    public string FilePath { get; }

    public BlobFile(string fileName, byte[] fileContent, string contentType, string parentDirectory = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(fileName, nameof(fileName));
        EnsureArg.IsNotNull(fileContent, nameof(fileContent));

        var extension = Path.GetExtension(fileName);

        EnsureArg.IsNotNullOrEmpty(extension, nameof(extension));

        FileName = GenerateFileName(extension);
        FileContent = fileContent;
        ContentType = contentType ?? "text/plain";
        ParentDirectory = parentDirectory;
        FilePath = string.IsNullOrWhiteSpace(parentDirectory) ? FileName : Path.Combine(ParentDirectory, FileName);
    }

    public bool HasParentDirectory()
    {
        return !string.IsNullOrWhiteSpace(ParentDirectory);
    }
}
