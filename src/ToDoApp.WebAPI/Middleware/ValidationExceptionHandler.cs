using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;

namespace ToDoApp.WebAPI.Middleware;

[ExcludeFromCodeCoverage]
public sealed class ValidationExceptionHandler(IProblemDetailsService thisProblemDetails) : IExceptionHandler
{
    #region Interfaces
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if(exception is ValidationException validation_exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            ValidationProblemDetails problem = new(
                validation_exception.Errors.GroupBy(e => e.PropertyName).ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                )
            )
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Validation failed"
            };
            if(problem.Errors.Count == 0)
            {
                problem.Detail = validation_exception.Message;
            }
            await thisProblemDetails.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem
            });
            return true;
        }
        return false;
    }
    #endregion
}