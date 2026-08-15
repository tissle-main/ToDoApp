namespace ToDoApp.Web.Features.Tasks.Dtos;

public sealed class TaskDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool Done { get; set; }
    public List<Guid> Categories { get; set; } = [];
}