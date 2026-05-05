namespace Nilearn.Application.Common.Exceptions;
public sealed class ConflictException : Exception
{
    public string EntityName { get; }

    public ConflictException(string entityName, string message)
        : base(message)
    {
        EntityName = entityName;
    }
}
