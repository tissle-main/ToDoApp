using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenResponse(string Email, string AccessToken, RefreshTokenDto RefreshToken);