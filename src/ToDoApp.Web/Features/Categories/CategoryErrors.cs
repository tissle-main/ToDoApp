using ErrorOr;

namespace ToDoApp.Web.Features.Categories;

public static class CategoryErrors
{
    public static Error NotFound()
    {
        return Error.NotFound("Category.NotFound", "One or more categories not found.");
    }
}