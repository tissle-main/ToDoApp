using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Dtos;

public sealed class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public bool Done { get; set; }
    public List<CategoryDto> Categories { get; set; } = [];
}