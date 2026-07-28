using Mediator;
using FluentResults;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed record class DeleteCategoryCommand(Guid Id) : IRequest<Result>;