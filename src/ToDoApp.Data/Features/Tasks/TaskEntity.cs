using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Shared.KeyedEntities;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Data.Features.Tasks;

public sealed class TaskEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; } //Interfaces
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
    public List<Task_Category_JoinEntity> Categories { get; set; } = [];
}