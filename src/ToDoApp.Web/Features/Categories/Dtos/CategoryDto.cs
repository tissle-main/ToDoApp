namespace ToDoApp.Web.Features.Categories.Dtos;

public sealed class CategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<Guid> Tasks { get; set; } = [];
}