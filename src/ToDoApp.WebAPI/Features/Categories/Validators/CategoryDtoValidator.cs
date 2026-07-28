using FluentValidation;
using ToDoApp.Data.Features.Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Validators;

public sealed class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        base.RuleFor(e => e.Name).MaximumLength(CategoryConstants.NameMaxLength).NotEmpty();
    }
}