using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Handlers;

public sealed record class UpdateTaskCommand(TaskDto Task) : IRequest<Result>;