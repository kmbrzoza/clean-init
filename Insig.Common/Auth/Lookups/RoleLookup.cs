using System.Collections.Generic;

namespace Insig.Common.Auth.Lookups;

public enum RoleEnum
{
    User = 1,
    Admin
}

public class RoleLookup
{
    public static Dictionary<int, string> Descriptions = new()
    {
        { (int)RoleEnum.User, "User" },
        { (int)RoleEnum.Admin, "Admin" }
    };
}