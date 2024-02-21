using System;

namespace Insig.Domain.Common;

public class AuditableEntity
{
    public long CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}