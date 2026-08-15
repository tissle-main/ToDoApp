using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Auth.Handlers.RemoveExpiredRefreshTokens;

public sealed record class RemoveExpiredRefreshTokensCommand : IDbTransactionBehaviorMessage, ICommand<ErrorOr<Unit>>;