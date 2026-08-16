namespace ToDoApp.Web.Features.Tasks_Categories;

public sealed class Task_Categories_FeatureProvider : FeatureProvider
{
    #region Base
    public override void UseMiddleware(WebApplication app)
    {
        if(app.Environment.IsEnvironment("Test"))
        {
            app.AddTask_Category_UpdateEndpoint();
        }
    }
    #endregion
}