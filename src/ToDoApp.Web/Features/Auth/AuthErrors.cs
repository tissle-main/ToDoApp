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
        return Error.Failure("Auth.InvalidPassword", "Invalid password.");
    }

    extension(IdentityResult thisResult)
    {
        public IEnumerable<Error> ToErrors()
        {
            if(thisResult.Succeeded)
            {
                return [];
            }
            return thisResult.Errors.Select(error => Error.Failure(error.Code, error.Description));
        }
    }
    extension(SignInResult thisResult)
    {
        public Error ToError()
        {
            if(thisResult.IsLockedOut)
            {
                return UserLockedOut();
            }
            if(thisResult.IsNotAllowed)
            {
                return UserNotAllowed();
            }
            if(thisResult.RequiresTwoFactor)
            {
                return Requires2FA();
            }
            return InvalidPassword();
        }
    }
}