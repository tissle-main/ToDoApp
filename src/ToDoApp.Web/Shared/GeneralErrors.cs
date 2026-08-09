using ErrorOr;

namespace ToDoApp.Web.Shared;

public static class GeneralErrors
{
    public static Error Unauthorized()
    {
        return Error.Unauthorized("General.Unauthorized", "User not authorized.");
    }
    public static Error ValidationFailure()
    {
        return Error.Validation("General.ValidationFailure", "One or more validation rules not satisfied.");
    }
}