using FluentValidation;

namespace ToDoApp.Web.Features.Auth.Handlers.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        base.RuleFor(e => e.Email).NotEmpty().EmailAddress();
        base.RuleFor(e => e.Password).NotEmpty().Matches(AuthConstants.PasswordRegex).WithMessage(AuthConstants.PasswordValidationMessage);
    }
}