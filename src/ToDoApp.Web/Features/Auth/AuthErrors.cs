using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Web.Features.Auth;

public static class AuthErrors
{
    public static Error UserExists()
    {
        return Error.Conflict("Auth.UserExists", "User already exists.");
    }
    public static Error UserNotFound()
    {
        return Error.NotFound("Auth.UserNotFound", "User not found.");
    }
    public static Error UserLockedOut()
    {
        return Error.Failure("Auth.UserLockedOut", "User locked out.");
    }
    public static Error UserNotAllowed()
    {
        return Error.Failure("Auth.UserNotAllowed", "User not allowed to sign in.");
    }
    public static Error Requires2FA()
    {
        return Error.Failure("Auth.Requires2FA", "Sign in requires 2FA.");
    }
    public static Error InvalidPassword()
    {
        return Error.Failure("Auth.InvalidPassword", "Entered invalid password.");
    }
    public static Error RefreshTokenNotFound()
    {
        return Error.NotFound("Auth.RefreshTokenNotFound", "Refresh token not found.");
    }
    public static Error RefreshTokenExpired()
    {
        return Error.Failure("Auth.RefreshTokenExpired", "Refresh token expired.");
    }

    public static IEnumerable<Error> ToErrors(this IdentityResult result)
    {
        if(result.Succeeded)
        {
            return [];
        }
        return result.Errors.Select(error => Error.Failure(error.Code, error.Description));
    }
    public static Error ToError(this SignInResult result)
    {
        if(result.IsLockedOut)
        {
            return AuthErrors.UserLockedOut();
        }
        if(result.IsNotAllowed)
        {
            return AuthErrors.UserNotAllowed();
        }
        if(result.RequiresTwoFactor)
        {
            return AuthErrors.Requires2FA();
        }
        return AuthErrors.InvalidPassword();
    }
}