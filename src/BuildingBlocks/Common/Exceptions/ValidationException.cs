using FluentValidation.Results;

namespace BuildingBlocks.Common.Exceptions;

public class ValidationException : BaseException
{
    public IEnumerable<ValidationFailure> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> errors)
        : base("Validation failed", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new List<ValidationFailure>();
    }
}
