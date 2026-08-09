using ErrorOr;
using Mediator;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class RefreshAccessTokenCommand(string RefreshToken) : ICommand<ErrorOr<RefreshAccessTokenResponse>>;