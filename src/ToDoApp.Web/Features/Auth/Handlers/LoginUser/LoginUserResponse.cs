using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserResponse(string Email, string AccessToken, RefreshTokenDto RefreshToken);