using ErrorOr;
using Mediator;
using System.Reflection;

namespace ToDoApp.Web.Shared.Behaviors;

public abstract class BehaviorBase<TMessage, TErrorOrValue> : IPipelineBehavior<TMessage, TErrorOrValue>
    where TMessage : IMessage
    where TErrorOrValue : IErrorOr
{
    #region Static
    protected static Func<List<Error>, TErrorOrValue> FromErrors { get; }

    static BehaviorBase()
    {
        Type errorType = typeof(TErrorOrValue);
        if(errorType.IsGenericType && errorType.GetGenericTypeDefinition() == typeof(ErrorOr<>))
        {
            Type typeArg = errorType.GenericTypeArguments[0];
            MethodInfo method = typeof(ErrorOrFactory).GetMethod(
                name: nameof(ErrorOrFactory.From),
                genericParameterCount: 1,
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                types: [typeof(List<Error>)]
            )!.MakeGenericMethod(typeArg);
            FromErrors = (Func<List<Error>, TErrorOrValue>)Delegate.CreateDelegate(typeof(Func<List<Error>, TErrorOrValue>), method);
        }
        else
        {
            throw new InvalidOperationException(
                $"Generic argument '{typeof(TErrorOrValue).Name}' " +
                $"of type '{typeof(BehaviorBase<TMessage, TErrorOrValue>).FullName}' " +
                $"must be '{typeof(IErrorOr<>).FullName}."
            );
        }
    }
    #endregion

    #region Instance
    public abstract ValueTask<TErrorOrValue> Handle(TMessage message, MessageHandlerDelegate<TMessage, TErrorOrValue> next, CancellationToken cancellationToken);
    #endregion
}