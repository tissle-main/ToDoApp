using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Handlers.GenerateTokens;

public sealed record class GenerateTokensResponse(string AccessToken, RefreshTokenDto RefreshToken);