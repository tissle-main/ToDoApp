using System.Diagnostics.CodeAnalysis;

namespace ToDoApp.Web.Features.Auth;

public static class AuthConstants
{
    public const int PasswordMinLength = 8;
    public const string PasswordValidationMessage = "Password must contain at least 8 characters, including uppercase, lowercase, and a number.";

    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string PasswordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{8,}$";
}