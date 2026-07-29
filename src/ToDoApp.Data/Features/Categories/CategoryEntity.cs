using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Data.Features.Categories;

public sealed class CategoryEntity
{
    //Value properties
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public ApplicationUser? User { get; set; }
    public List<Task_Category_JoinEntity> Tasks { get; set; } = [];
}