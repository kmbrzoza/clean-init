using System.IO;

namespace Insig.Infrastructure.FileProcessing.Models;

public interface IStreamBlobFile
{
    string FileName { get; }
    Stream FileStream { get; }
    string ContentType { get; }
    long Length { get; }
    string ParentDirectory { get; }
    string FilePath { get; }

    bool HasParentDirectory();
}
