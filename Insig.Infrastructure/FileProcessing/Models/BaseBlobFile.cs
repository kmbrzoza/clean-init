using System;

namespace Insig.Infrastructure.FileProcessing.Models;

public abstract class BaseBlobFile
{
    protected string GenerateFileName(string fileExtension)
    {
        return $"{Guid.NewGuid()}{fileExtension.ToLower()}";
    }
}
