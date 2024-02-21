namespace Insig.Domain.Common;

public abstract record RecordWithValidation
{
    protected RecordWithValidation()
    {
        Validate();
    }

    protected abstract void Validate();
}