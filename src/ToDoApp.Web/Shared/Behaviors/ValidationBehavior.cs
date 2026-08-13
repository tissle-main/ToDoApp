using ErrorOr;
using Mediator;
using FluentValidation;
using FluentValidation.Results;

namespace ToDoApp.Web.Shared.Behaviors;

public sealed class ValidationBehavior<TMessage, TErrorOrValue>(IEnumerable<IValidator<TMessage>> thisValidators) : BehaviorBase<TMessage, TErrorOrValue>
    where TMessage : IMessage
    where TErrorOrValue : IErrorOr 
{
    #region Base
    public override async ValueTask<TErrorOrValue> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TErrorOrValue> next,
        CancellationToken cancellationToken
    )
    {
        if(!thisValidators.Any())
        {
            return await next(message, cancellationToken);
        }
        ValidationContext<TMessage> context = new(message);
        ValidationResult[] results = await Task.WhenAll(
            thisValidators.Select(v => v.ValidateAsync(context, cancellationToken))
        );
        List<Error> failures = results.SelectMany(r => r.Errors).Select(error => Error.Validation(error.ErrorCode, error.ErrorMessage)).ToList();
        if(failures.Count != 0)
        {
            return FromErrors(failures);
        }
        return await next(message, cancellationToken);
    }
    #endregion
}