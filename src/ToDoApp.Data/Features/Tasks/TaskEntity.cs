using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Data.Features.Tasks;

public sealed class TaskEntity
{
    //Value properties
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool Done { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public ApplicationUser? User { get; set; }
    public List<Task_Category_JoinEntity> Categories { get; set; } = [];
}