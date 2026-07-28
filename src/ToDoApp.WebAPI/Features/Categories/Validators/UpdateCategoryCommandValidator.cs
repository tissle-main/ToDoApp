using FluentValidation;
using ToDoApp.WebAPI.Features.Categories.Handlers;

namespace ToDoApp.WebAPI.Features.Categories.Validators;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        base.RuleFor(e => e.Category).SetValidator(new CategoryDtoValidator());
    }
}