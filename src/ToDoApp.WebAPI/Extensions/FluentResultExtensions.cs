using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebUtilities;

namespace ToDoApp.WebAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class FluentResultExtensions
{
    private const string StatusCodeKey = "StatusCode";

    private static ProblemDetails ToProblemDetails(IEnumerable<IError> errors, int statusCode)
    {
        return new ProblemDetails()
        {
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = string.Join(' ', errors.Select(e => e.Message)),
            Status = statusCode
        };
    }
    public static Result LogTo<TLogger>(this Result result, ILogger<TLogger> logger, LogLevel logLevel = LogLevel.Error)
    {
        string msg = string.Join(' ', result.Reasons.Select(r => r.Message));
#pragma warning disable CA2254
        logger.Log(logLevel, msg);
#pragma warning restore CA2254
        return result;
    }
    public static Result<TResult> LogTo<TResult, TLogger>(this Result<TResult> result, ILogger<TLogger> logger, LogLevel logLevel = LogLevel.Error)
    {
        string msg = string.Join(' ', result.Reasons.Select(r => r.Message));
#pragma warning disable CA2254
        logger.Log(logLevel, msg);
#pragma warning restore CA2254
        return result;
    }
    public static Result WithStatusCode(this Result result, int statusCode)
    {
        if(result.IsSuccess)
        {
            Success success = new(ReasonPhrases.GetReasonPhrase(statusCode));
            success = success.WithMetadata(StatusCodeKey, statusCode);
            return result.WithSuccess(success);
        }
        else
        {
            Error error = new(ReasonPhrases.GetReasonPhrase(statusCode));
            error = error.WithMetadata(StatusCodeKey, statusCode);
            return result.WithError(error);
        }
    }
    public static Result<T> WithStatusCode<T>(this Result<T> result, int statusCode)
    {
        if(result.IsSuccess)
        {
            Success success = new(ReasonPhrases.GetReasonPhrase(statusCode));
            success = success.WithMetadata(StatusCodeKey, statusCode);
            return result.WithSuccess(success);
        }
        else
        {
            Error error = new(ReasonPhrases.GetReasonPhrase(statusCode));
            error = error.WithMetadata(StatusCodeKey, statusCode);
            return result.WithError(error);
        }
    }
    public static IResult ToMvcResult(this Result result)
    {
        if(result.Reasons.FirstOrDefault(r => r.HasMetadataKey(StatusCodeKey)) is IReason reason)
        {
            int statusCode = (int)reason.Metadata[StatusCodeKey];
            switch(statusCode)
            {
                case StatusCodes.Status400BadRequest:
                {
                    return Results.BadRequest(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status401Unauthorized:
                {
                    return Results.Unauthorized();
                }
                case StatusCodes.Status403Forbidden:
                {
                    return Results.Forbid();
                }
                case StatusCodes.Status404NotFound:
                {
                    return Results.NotFound(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status409Conflict:
                {
                    return Results.Conflict(ToProblemDetails(result.Errors, statusCode));
                }
                default:
                {
                    return Results.StatusCode(statusCode);
                }
            }
        }
        else
        {
            if(result.IsSuccess)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.BadRequest(ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest));
            }
        }
    }
    public static IResult ToMvcResult<T>(this Result<T> result)
    {
        if(result.Reasons.FirstOrDefault(r => r.HasMetadataKey(StatusCodeKey)) is IReason reason)
        {
            int statusCode = (int)reason.Metadata[StatusCodeKey];
            switch(statusCode)
            {
                case StatusCodes.Status400BadRequest:
                {
                    return Results.BadRequest(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status401Unauthorized:
                {
                    return Results.Unauthorized();
                }
                case StatusCodes.Status403Forbidden:
                {
                    return Results.Forbid();
                }
                case StatusCodes.Status404NotFound:
                {
                    return Results.NotFound(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status409Conflict:
                {
                    return Results.Conflict(ToProblemDetails(result.Errors, statusCode));
                }
                default:
                {
                    return Results.StatusCode(statusCode);
                }
            }
        }
        else
        {
            if(result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return Results.BadRequest(ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest));
            }
        }
    }
}