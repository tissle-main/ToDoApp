using ToDoApp.Web.Features.Categories.Handlers.GetCategories;
using ToDoApp.Web.Features.Categories.Handlers.CreateCategory;
using ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;
using ToDoApp.Web.Features.Categories.Handlers.DeleteCategories;

namespace ToDoApp.Web.Features.Categories;

public sealed class CategoryFeatureProvider : FeatureProvider
{
    #region Base
    public override void UseMiddleware(WebApplication app)
    {
        app.AddGetCategoriesEndpoint();
        app.AddCreateCategoryEndpoint();
        app.AddUpdateCategoryEndpoint();
        app.AddDeleteCategoriesEndpoint();
    }
    #endregion
}