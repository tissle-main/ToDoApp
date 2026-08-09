using FluentValidation;
using ToDoApp.Web.Features.Auth.Handlers;

namespace ToDoApp.Web.Features.Auth.Validators;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        base.RuleFor(e => e.Email).NotEmpty().EmailAddress();
        base.RuleFor(e => e.Password).NotEmpty().Matches(AuthConstants.PasswordRegex).WithMessage(AuthConstants.PasswordValidationMessage);
    }
}