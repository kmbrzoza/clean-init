using System;

namespace Insig.Common.Infrastructure.DateTimeProvider;

#pragma warning disable RS0030
public class DateTimeProvider
{
    public static DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}
