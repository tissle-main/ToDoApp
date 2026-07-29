using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed record class DeleteCategoriesCommand(Guid[] Ids) : IRequest<Result>;