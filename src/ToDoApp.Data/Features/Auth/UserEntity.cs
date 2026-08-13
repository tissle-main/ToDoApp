using ToDoApp.Data.Features.Tasks;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Data.Features.Auth;

public sealed class UserEntity : IdentityUser<Guid>
{
    //Navigation properties
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = [];
    public List<TaskEntity> Tasks { get; set; } = [];
    public List<CategoryEntity> Categories { get; set; } = [];
}