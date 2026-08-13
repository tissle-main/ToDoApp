using FluentValidation;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;

namespace ToDoApp.Web.Features.Categories.Validators;

public sealed class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        base.RuleFor(dto => dto.Name).NotEmpty().MaximumLength(CategoryEntityConstants.NameMaxLength);
    }
}