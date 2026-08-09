namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class RefreshAccessTokenResponse(string Email, string AccessToken, string RefreshToken);