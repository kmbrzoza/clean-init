using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Insig.Common.Lookups;

public enum NotificationStatusEnum
{
    Pending = 1,
    Completed
}

public class NotificationStatusLookup
{
    public static ReadOnlyDictionary<int, string> Descriptions = new Dictionary<int, string>()
    {
        { (int)NotificationStatusEnum.Pending, "Pending" },
        { (int)NotificationStatusEnum.Completed, "Completed" }
    }.AsReadOnly();
}