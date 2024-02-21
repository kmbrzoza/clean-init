namespace Insig.Infrastructure.FileProcessing.Models;

public interface IBlobFile
{
    string FileName { get; }
    byte[] FileContent { get; }
    string ContentType { get; }
    public string ParentDirectory { get; }
    public string FilePath { get; }

    bool HasParentDirectory();
}