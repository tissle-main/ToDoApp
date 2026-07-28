using Mediator;
using FluentResults;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Categories.Handlers;

public sealed record class GetAllCategoriesQuery() : IRequest<Result<IEnumerable<CategoryDto>>>;