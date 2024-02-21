using EnsureThat;

namespace Insig.Infrastructure.FileProcessing.Containers;

public abstract class StorageContainer
{
    public string Name { get; }
    public bool IsPrivate { get; }

    protected StorageContainer(string containerName, bool isPrivate = false)
    {
        EnsureArg.IsNotNullOrWhiteSpace(containerName, nameof(containerName));

        Name = containerName;
        IsPrivate = isPrivate;
    }
}
