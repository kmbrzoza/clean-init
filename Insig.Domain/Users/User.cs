using System;
using EnsureThat;
using Insig.Common.Auth.Lookups;
using Insig.Common.Infrastructure.DateTimeProvider;

namespace Insig.Domain.Users;

public class User
{
    public User(string sub, string email)
    {
        EnsureArg.IsNotNullOrWhiteSpace(sub, nameof(sub));
        EnsureArg.IsNotNullOrWhiteSpace(email, nameof(email));

        Sub = sub;
        Email = email;
        RoleId = (int)RoleEnum.User;
        CreatedOn = DateTimeProvider.UtcNow();
    }

    public long Id { get; }
    public string Sub { get; }
    public string Email { get; private set; }
    public int RoleId { get; private set; }

    public DateTime CreatedOn { get; set; }
}