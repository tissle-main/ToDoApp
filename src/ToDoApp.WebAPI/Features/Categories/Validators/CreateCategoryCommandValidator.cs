using FluentValidation;
using ToDoApp.WebAPI.Features.Categories.Handlers;

namespace ToDoApp.WebAPI.Features.Categories.Validators;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        base.RuleFor(e => e.Category).SetValidator(new CategoryDtoValidator());
    }
}