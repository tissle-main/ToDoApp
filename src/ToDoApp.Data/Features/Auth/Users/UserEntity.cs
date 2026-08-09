using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Data.Features.Auth.Users;

public sealed class UserEntity : IdentityUser<Guid>
{
    //Navigation properties
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = [];
}