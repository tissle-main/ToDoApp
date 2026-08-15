using ErrorOr;
using Mediator;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenCommand(string RefreshToken) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<RefreshAccessTokenResponse>>;