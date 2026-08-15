using ToDoApp.Web.Features;

namespace ToDoApp.Web.Shared.Behaviors.Authorized;

public sealed class AuthorizedBehaviorFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
    }
    #endregion
}