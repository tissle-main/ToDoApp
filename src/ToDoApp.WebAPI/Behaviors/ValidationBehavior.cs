using Mediator;
using FluentValidation;
using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace ToDoApp.WebAPI.Behaviors;

[ExcludeFromCodeCoverage]
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> thisValidators
) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IMessage
{
    #region Interfaces
    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if(!thisValidators.Any())
        {
            return await next(message, cancellationToken);
        }
        ValidationContext<TRequest> context = new(message);
        ValidationResult[] results = await Task.WhenAll(
            thisValidators.Select(v => v.ValidateAsync(context, cancellationToken))
        );
        List<ValidationFailure> failures = results.SelectMany(r => r.Errors).ToList();
        if(failures.Count != 0)
        {
            throw new ValidationException(failures);
        }
        return await next(message, cancellationToken);
    }
    #endregion
}