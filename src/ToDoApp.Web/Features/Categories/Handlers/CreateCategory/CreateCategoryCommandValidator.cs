using FluentValidation;
using ToDoApp.Web.Features.Categories.Dtos;

namespace ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        base.RuleFor(e => e.Category).SetValidator(new CategoryDtoValidator());
    }
}