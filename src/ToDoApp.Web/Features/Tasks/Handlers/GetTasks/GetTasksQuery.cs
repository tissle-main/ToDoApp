using ToDoApp.Web.Features.Tasks.Dtos;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasks;

public sealed record class GetTasksResponse(IEnumerable<TaskDto> Tasks);