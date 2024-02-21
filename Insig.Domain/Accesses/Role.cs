using EnsureThat;

namespace Insig.Domain.Accesses;

public class Role
{
    public Role(string name)
    {
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        Name = name;
    }

    public Role(int id, string name) : this(name)
    {
        Id = id;
    }

    public int Id { get; }
    public string Name { get; private set; }
}
