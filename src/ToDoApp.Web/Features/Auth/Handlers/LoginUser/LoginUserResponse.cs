namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserResponse(string Email, string AccessToken);