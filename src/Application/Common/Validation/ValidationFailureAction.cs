using ValidationException = CleanArchitectureWebApi.Application.Common.Exceptions.ValidationException;
using FluentValidation.Results;
using Wolverine.FluentValidation;

namespace CleanArchitectureWebApi.Application.Common.Validation;

/// <summary>
/// Replaces the default Wolverine.FluentValidation failure action (which throws FluentValidation's own
/// ValidationException) so validation failures surface as the application's ValidationException.
/// </summary>
public class ValidationFailureAction<T> : IFailureAction<T>
{
    public void Throw(T message, IReadOnlyList<ValidationFailure> failures)
    {
        throw new ValidationException(failures);
    }
}
