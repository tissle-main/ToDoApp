using FluentValidation;
using ToDoApp.Web.Features.Categories.Handlers;

namespace ToDoApp.Web.Features.Categories.Validators;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        base.RuleFor(e => e.Category).SetValidator(new CategoryDtoValidator());
    }
}