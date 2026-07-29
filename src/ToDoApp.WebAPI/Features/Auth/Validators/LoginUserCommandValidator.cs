using FluentValidation;
using ToDoApp.WebAPI.Features.Auth.Handlers;

namespace ToDoApp.WebAPI.Features.Auth.Validators;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        base.RuleFor(e => e.Email).EmailAddress();
        base.RuleFor(e => e.Password).Matches(AuthValidatorsConstants.PasswordRegex);
    }
}