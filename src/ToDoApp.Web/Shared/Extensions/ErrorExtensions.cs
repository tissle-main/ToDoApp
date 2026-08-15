using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Web.Shared.Extensions;

public static class ErrorExtensions
{
    extension(Error thisError)
    {
        private ProblemDetails ToProblemDetails()
        {
            return new ProblemDetails()
            {
                Status = thisError.Type switch
                {
                    ErrorType.Failure => StatusCodes.Status400BadRequest,
                    ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status500InternalServerError
                },
                Title = thisError.Code,
                Detail = thisError.Description,
            };
        }
    }
    extension(List<Error>? thisErrors)
    {
        private ProblemDetails ToProblemDetails()
        {
            if(thisErrors is null || thisErrors.Count == 0)
            {
                thisErrors = [Error.Unexpected()];
            }
            if(thisErrors.All(error => error.Type is ErrorType.Validation))
            {
                Error validationError = Error.Validation();
                return new HttpValidationProblemDetails()
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = validationError.Code,
                    Detail = validationError.Description,
                    Errors = thisErrors.Select(error =>
                    {
                        return new KeyValuePair<string, string[]>(error.Code, [error.Description]);
                    }).ToDictionary()
                };
            }
            ProblemDetails problemDetails = thisErrors.First().ToProblemDetails();
            problemDetails.Extensions = thisErrors.Skip(1).Select((error, index) =>
            {
                return new KeyValuePair<string, object?>(index.ToString(), error.ToProblemDetails());
            }).ToDictionary();
            return problemDetails;
        }
    }
    extension<TValue>(ErrorOr<TValue> thisErrorOrValue)
    {
        public IResult ToHttpResult()
        {
            if(thisErrorOrValue.IsSuccess)
            {
                return typeof(TValue) == typeof(Unit) ? Results.NoContent() : Results.Ok(thisErrorOrValue.Value);
            }
            return Results.Problem(thisErrorOrValue.Errors.ToProblemDetails());
        }
    }
}