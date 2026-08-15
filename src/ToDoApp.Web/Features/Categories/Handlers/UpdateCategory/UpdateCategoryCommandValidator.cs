using FluentValidation;
using ToDoApp.Web.Features.Categories.Dtos;

namespace ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        base.RuleFor(e => e.Category).SetValidator(new CategoryDtoValidator());
    }
}