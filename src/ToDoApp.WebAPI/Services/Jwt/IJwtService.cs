using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.WebAPI.Services.Jwt;

public interface IJwtService
{
    public abstract Task<string> GenerateTokenAsync(ApplicationUser user);
}