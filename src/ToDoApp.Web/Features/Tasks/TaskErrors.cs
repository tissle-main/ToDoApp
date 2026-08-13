using ErrorOr;

namespace ToDoApp.Web.Features.Tasks;

public static class TaskErrors
{
    public static Error NotFound()
    {
        return Error.NotFound("Task.NotFound", "One or more tasks not found.");
    }
}