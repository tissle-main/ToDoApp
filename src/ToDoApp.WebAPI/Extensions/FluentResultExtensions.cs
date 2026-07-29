using FluentResults;
using ToDoApp.WebAPI.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.WebUtilities;

namespace ToDoApp.WebAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class FluentResultExtensions
{
    private const string StatusCodeKey = "StatusCode";

    private static ProblemDetails ToProblemDetails(IEnumerable<IError> errors, int statusCode)
    {
        IEnumerable<string> msgs = errors.Select(e => e.Message).Where(m => !string.IsNullOrWhiteSpace(m));
        return new ProblemDetails()
        {
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = string.Join(' ', msgs),
            Status = statusCode
        };
    }
    public static Result LogTo<TLogger>(this Result result, ILogger<TLogger> logger, LogLevel logLevel = LogLevel.Error)
    {
        IEnumerable<string> errors = result.Reasons.Select(r => r.Message).Where(m => !string.IsNullOrWhiteSpace(m));
        string msg = string.Join(' ', errors);
#pragma warning disable CA2254
        logger.Log(logLevel, msg);
#pragma warning restore CA2254
        return result;
    }
    public static Result<TResult> LogTo<TResult, TLogger>(this Result<TResult> result, ILogger<TLogger> logger, LogLevel logLevel = LogLevel.Error)
    {
        IEnumerable<string> errors = result.Reasons.Select(r => r.Message).Where(m => !string.IsNullOrWhiteSpace(m));
        string msg = string.Join(' ', errors);
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
    public static IActionResult ToActionResult(this Result result)
    {
        if(result.Reasons.FirstOrDefault(r => r.HasMetadataKey(StatusCodeKey)) is IReason reason)
        {
            int statusCode = (int)reason.Metadata[StatusCodeKey];
            switch(statusCode)
            {
                case StatusCodes.Status400BadRequest:
                {
                    return new BadRequestObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status401Unauthorized:
                {
                    return new UnauthorizedObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status403Forbidden:
                {
                    return new ForbidResult();
                }
                case StatusCodes.Status404NotFound:
                {
                    return new NotFoundObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status409Conflict:
                {
                    return new ConflictObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                default:
                {
                    return new StatusCodeResult(statusCode);
                }
            }
        }
        else
        {
            if(result.IsSuccess)
            {
                return new NoContentResult();
            }
            else
            {
                return new BadRequestObjectResult(ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest));
            }
        }
    }
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if(result.Reasons.FirstOrDefault(r => r.HasMetadataKey(StatusCodeKey)) is IReason reason)
        {
            int statusCode = (int)reason.Metadata[StatusCodeKey];
            switch(statusCode)
            {
                case StatusCodes.Status400BadRequest:
                {
                    return new BadRequestObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status401Unauthorized:
                {
                    return new UnauthorizedObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status403Forbidden:
                {
                    return new ForbidResult();
                }
                case StatusCodes.Status404NotFound:
                {
                    return new NotFoundObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                case StatusCodes.Status409Conflict:
                {
                    return new ConflictObjectResult(ToProblemDetails(result.Errors, statusCode));
                }
                default:
                {
                    return new StatusCodeResult(statusCode);
                }
            }
        }
        else
        {
            if(result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }
            else
            {
                return new BadRequestObjectResult(ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest));
            }
        }
    }
    public static Result ToFluentResult(this IdentityResult identityResult)
    {
        if(identityResult.Succeeded)
        {
            return Result.Ok();
        }
        return Result.Fail(identityResult.Errors.Select(e => e.Description));
    }
    public static Result<T?> ToFluentResult<T>(this IdentityResult identityResult, T? value = default)
    {
        if(identityResult.Succeeded)
        {
            return Result.Ok(value);
        }
        return Result.Fail<T?>(identityResult.Errors.Select(e => e.Description));
    }
    public static Result ToFluentResult(this Microsoft.AspNetCore.Identity.SignInResult signInResult)
    {
        if(signInResult.Succeeded)
        {
            return Result.Ok();
        }
        if(signInResult.IsLockedOut || signInResult.IsNotAllowed || signInResult.RequiresTwoFactor)
        {
            return Result.Fail(signInResult.ToString());
        }
        return Result.Fail(ErrorMessages.InvalidPassword);
    }
    public static Result<T?> ToFluentResult<T>(this Microsoft.AspNetCore.Identity.SignInResult signInResult, T? value = default)
    {
        if(signInResult.Succeeded)
        {
            return Result.Ok(value);
        }
        if(signInResult.IsLockedOut || signInResult.IsNotAllowed || signInResult.RequiresTwoFactor)
        {
            return Result.Fail<T?>(signInResult.ToString());
        }
        return Result.Fail<T?>(ErrorMessages.InvalidPassword);
    }
}