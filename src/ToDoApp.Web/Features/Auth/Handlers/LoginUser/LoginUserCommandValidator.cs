using FluentValidation;

namespace ToDoApp.Web.Features.Auth.Handlers.LoginUser;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        base.RuleFor(e => e.Email).NotEmpty().EmailAddress();
        base.RuleFor(e => e.Password).NotEmpty().Matches(AuthConstants.PasswordRegex).WithMessage(AuthConstants.PasswordValidationMessage);
    }
}