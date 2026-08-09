namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class GenerateTokensResponse(string AccessToken, string RefreshToken);