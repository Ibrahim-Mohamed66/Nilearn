namespace Nilearn.Application.Common.Exceptions;
public sealed class BadRequestException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public BadRequestException(string message)
        : base(message)
    {
        Errors = new List<string> { message }.AsReadOnly();
    }

    public BadRequestException(string message, IEnumerable<string> errors)
        : base(message)
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
