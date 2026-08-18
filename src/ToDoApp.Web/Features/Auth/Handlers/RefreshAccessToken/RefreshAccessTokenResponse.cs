using ToDoApp.Web.Features.Auth.Dtos.Users;

namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenResponse(UserDto User, string AccessToken);