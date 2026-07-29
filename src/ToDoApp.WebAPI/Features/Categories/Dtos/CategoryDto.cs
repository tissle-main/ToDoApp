namespace ToDoApp.WebAPI.Features.Categories.Dtos;

public sealed class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<Guid> Tasks { get; set; } = [];
}