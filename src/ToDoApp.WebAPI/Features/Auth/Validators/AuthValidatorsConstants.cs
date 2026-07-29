using System.Diagnostics.CodeAnalysis;

namespace ToDoApp.WebAPI.Features.Auth.Validators;

public static class AuthValidatorsConstants
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string PasswordRegex = "^[A-Za-z0-9]{8,}$";
}