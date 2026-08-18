using ToDoApp.Web.Features.Auth.Dtos.Users;

namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserResponse(UserDto User, string AccessToken);