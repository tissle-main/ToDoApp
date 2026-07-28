using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Dtos;

public sealed class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<TaskDto> Tasks { get; set; } = [];
}