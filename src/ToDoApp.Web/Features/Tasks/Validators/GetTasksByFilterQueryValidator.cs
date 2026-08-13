using FluentValidation;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Tasks.Handlers;

namespace ToDoApp.Web.Features.Tasks.Validators;

public sealed class GetTasksByFilterQueryValidator : AbstractValidator<GetTasksByFilterQuery>
{
    public GetTasksByFilterQueryValidator()
    {
        base.RuleFor(e => e.Search).MaximumLength(TaskEntityConstants.DescriptionMaxLength);
        base.RuleFor(e => e.Category).MaximumLength(CategoryEntityConstants.NameMaxLength);
        base.RuleFor(e => e.Skip).GreaterThanOrEqualTo(0).When(e => e.Skip is not null);
        base.RuleFor(e => e.Take).GreaterThanOrEqualTo(0).When(e => e.Take is not null);
    }
}