using ErrorOr;
using Mediator;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Shared.Behaviors.DbTransaction;

namespace ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;

public sealed record class GenerateTokensCommand(UserEntity User) : IDbTransactionBehaviorMessage, ICommand<ErrorOr<GenerateTokensResponse>>;