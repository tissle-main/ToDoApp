using ToDoApp.Web.Features.Tasks.Handlers.GetTasks;
using ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;
using ToDoApp.Web.Features.Tasks.Handlers.CreateTask;
using ToDoApp.Web.Features.Tasks.Handlers.DeleteTasks;
using ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

namespace ToDoApp.Web.Features.Tasks;

public sealed class TaskFeatureProvider : FeatureProvider
{
    #region Base
    public override void UseMiddleware(WebApplication app)
    {
        app.AddGetTasksEndpoint();
        app.AddGetTasksByFilterEndpoint();
        app.AddCreateTaskEndpoint();
        app.AddUpdateTaskEndpoint();
        app.AddDeleteTasksEndpoint();
    }
    #endregion
}