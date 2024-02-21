using System.IO;
using EnsureThat;

namespace Insig.Infrastructure.FileProcessing.Models;

public class StreamBlobFile : BaseBlobFile, IStreamBlobFile
{
    public string FileName { get; }
    public Stream FileStream { get; }
    public string ContentType { get; }
    public long Length { get; }
    public string ParentDirectory { get; }
    public string FilePath { get; }

    public StreamBlobFile(string fileName, Stream fileStream, string contentType, long length, string parentDirectory = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(fileName, nameof(fileName));
        EnsureArg.IsNotNull(fileStream, nameof(fileStream));

        var extension = Path.GetExtension(fileName);

        EnsureArg.IsNotNullOrEmpty(extension, nameof(extension));

        FileName = GenerateFileName(extension);
        FileStream = fileStream;
        ContentType = contentType ?? "text/plain";
        Length = length;
        ParentDirectory = parentDirectory;
        FilePath = string.IsNullOrWhiteSpace(parentDirectory) ? FileName : Path.Combine(ParentDirectory, FileName);
    }

    public bool HasParentDirectory()
    {
        return !string.IsNullOrWhiteSpace(ParentDirectory);
    }
}
