using FluentValidation;
using ToDoApp.WebAPI.Features.Tasks.Handlers;

namespace ToDoApp.WebAPI.Features.Tasks.Validators;

public sealed class GetTasksQueryValidator : AbstractValidator<GetTasksByFilterQuery>
{
    public GetTasksQueryValidator()
    {
        base.RuleFor(e => e.From).GreaterThanOrEqualTo(0);
        base.RuleFor(e => e.Count).GreaterThanOrEqualTo(0);
    }
}