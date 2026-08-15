using FluentValidation;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Web.Features.Categories.Dtos;

public sealed class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        base.RuleFor(dto => dto.Name).NotEmpty().MaximumLength(CategoryEntityConstants.NameMaxLength);
    }
}