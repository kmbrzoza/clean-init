using EnsureThat;

namespace Insig.Domain.Lookups;

public abstract class LookupType
{
    public int Id { get; set; }
    public string Name { get; set; }

    protected LookupType() { }

    protected LookupType(int id, string name)
    {
        EnsureArg.IsNotDefault(id, nameof(id));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        Id = id;
        Name = name;
    }
}