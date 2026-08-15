using FluentValidation;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

public sealed class GetTasksByFilterQueryValidator : AbstractValidator<GetTasksByFilterQuery>
{
    public GetTasksByFilterQueryValidator()
    {
        base.RuleFor(e => e.Skip).GreaterThanOrEqualTo(0).When(e => e.Skip is not null);
        base.RuleFor(e => e.Take).GreaterThanOrEqualTo(0).When(e => e.Take is not null);
    }
}