using FluentValidation;
using ToDoApp.Web.Features.Categories.Handlers;

namespace ToDoApp.Web.Features.Categories.Validators;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        base.RuleFor(e => e.Category).SetValidator(new CategoryDtoValidator());
    }
}