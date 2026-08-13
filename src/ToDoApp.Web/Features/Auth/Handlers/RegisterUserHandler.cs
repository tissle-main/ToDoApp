using Bogus;
using ErrorOr;
using Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Data.Features.Auth;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Web.Shared.Extensions;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class RegisterUserHandler(UserManager<UserEntity> thisUserManager) : ICommandHandler<RegisterUserCommand, ErrorOr<Unit>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<Unit>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if(await thisUserManager.FindByEmailAsync(command.Email) is not null)
        {
            return AuthErrors.UserExists();
        }
        UserEntity user = new()
        {
            Email = command.Email,
            UserName = command.Email
        };
        IdentityResult result = await thisUserManager.CreateAsync(user, command.Password);
        if(!result.Succeeded)
        {
            return result.ToErrors().ToList();
        }
        return Unit.Value;
    }
    #endregion
}
public sealed record class RegisterUserCommand(string Email, string Password) : ICommand<ErrorOr<Unit>>;
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        base.RuleFor(e => e.Email).NotEmpty().EmailAddress();
        base.RuleFor(e => e.Password).NotEmpty().Matches(AuthConstants.PasswordRegex).WithMessage(AuthConstants.PasswordValidationMessage);
    }
}
public static class RegisterUserCommandFaker
{
    public static Faker<RegisterUserCommand> ValidInstance(this Faker<RegisterUserCommand> faker)
    {
        return faker.CustomInstantiator(g =>
        {
            string email = g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName);
            string password = g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Za-z0-9]", prefix: "Aa0");
            return new RegisterUserCommand(email, password);
        });
    }
    public static Faker<RegisterUserCommand> WithEmail(this Faker<RegisterUserCommand> faker, string email)
    {
        return faker.RuleFor(c => c.Email, g => email);
    }
    public static Faker<RegisterUserCommand> WithPassword(this Faker<RegisterUserCommand> faker, string password)
    {
        return faker.RuleFor(c => c.Password, g => password);
    }
    public static Faker<RegisterUserCommand> WithInvalidEmail(this Faker<RegisterUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Email,
            g => g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName).Replace("@", "")
        );
    }
    public static Faker<RegisterUserCommand> WithTooShortPassword(this Faker<RegisterUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength - 1, regexPattern: "[A-Za-z0-9]", prefix: "Aa0")
        );
    }
    public static Faker<RegisterUserCommand> WithPasswordWithoutUppercaseLetters(this Faker<RegisterUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[a-z0-9]", prefix: "a0")
        );
    }
    public static Faker<RegisterUserCommand> WithPasswordWithoutLowercaseLetters(this Faker<RegisterUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Z0-9]", prefix: "A0")
        );
    }
    public static Faker<RegisterUserCommand> WithPasswordWithoutDigits(this Faker<RegisterUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Za-z]", prefix: "Aa")
        );
    }
}
public static class RegisterUserEndpoint
{
    public const string Url = "/auth/register";

    public static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static void AddRegisterUserEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(Url, RegisterUser)
            .WithName(nameof(RegisterUser))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
    }
    public static async ValueTask<HttpResponseMessage> SendRegisterUserAsync(
        this HttpClient httpClient,
        RegisterUserCommand command,
        CancellationToken cancellationToken
    )
    {
        return await HttpClientJsonExtensions.PostAsJsonAsync(httpClient, Url, command, cancellationToken);
    }
}