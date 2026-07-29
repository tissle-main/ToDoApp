using ToDoApp.Data.Features.Tasks;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Data.Features.Auth.Users;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    //Navigation properties
    public List<TaskEntity> Tasks { get; set; } = [];
    public List<CategoryEntity> Categories { get; set; } = [];
}