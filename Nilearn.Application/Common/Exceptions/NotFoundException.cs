namespace Nilearn.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public string EntityName { get; }
    public object? EntityKey { get; }

    public NotFoundException(string entityName, object? entityKey = null)
        : base($"{entityName} was not found." +
               (entityKey is not null ? $" Key: {entityKey}" : string.Empty))
    {
        EntityName = entityName;
        EntityKey = entityKey;
    }
}
