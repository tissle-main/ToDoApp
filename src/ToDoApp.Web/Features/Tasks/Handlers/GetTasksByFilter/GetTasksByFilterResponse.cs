using ToDoApp.Web.Features.Tasks.Dtos;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

public sealed record class GetTasksByFilterResponse(IEnumerable<TaskDto> Tasks);