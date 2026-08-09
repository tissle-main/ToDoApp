namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed record class LoginUserResponse(string Email, string AccessToken, string RefreshToken);