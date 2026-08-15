using ToDoApp.Web.Features.Categories.Dtos;

namespace ToDoApp.Web.Features.Categories.Handlers.GetCategories;

public sealed record class GetCategoriesResponse(IEnumerable<CategoryDto> Categories);