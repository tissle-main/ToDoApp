namespace ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenResponse(string Email, string AccessToken);