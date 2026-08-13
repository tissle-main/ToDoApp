using Bogus;
using ErrorOr;
using Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Data.Features.Auth;
using ToDoApp.Web.Shared.Behaviors;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using ToDoApp.Web.Features.Auth.Dtos;
using LoginResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ToDoApp.Web.Features.Auth.Handlers;

public sealed class LoginUserHandler(
    SignInManager<UserEntity> thisSignInManager,
    IMediator thisMediator
) : ICommandHandler<LoginUserCommand, ErrorOr<LoginUserResponse>>
{
    #region Interfaces
    public async ValueTask<ErrorOr<LoginUserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        if(await thisSignInManager.UserManager.FindByEmailAsync(command.Email) is not UserEntity user)
        {
            return AuthErrors.UserNotFound();
        }
        LoginResult result = await thisSignInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return result.ToError();
        }
        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(user)
        {
            SaveDatabase = false
        }, cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            return new LoginUserResponse(user.Email!, tokens.AccessToken, tokens.RefreshToken);
        });    
    }
    #endregion
}
public sealed record class LoginUserCommand(string Email, string Password) : IDbSaveMessage, ICommand<ErrorOr<LoginUserResponse>>;
public sealed record class LoginUserResponse(string Email, string AccessToken, RefreshTokenDto RefreshToken);
public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        base.RuleFor(e => e.Email).NotEmpty().EmailAddress();
        base.RuleFor(e => e.Password).NotEmpty().Matches(AuthConstants.PasswordRegex).WithMessage(AuthConstants.PasswordValidationMessage);
    }
}
public static class LoginUserCommandFaker
{
    public static Faker<LoginUserCommand> ValidInstance(this Faker<LoginUserCommand> faker)
    {
        return faker.CustomInstantiator(g =>
        {
            string email = g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName);
            string password = g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Za-z0-9]", prefix: "Aa0");
            return new LoginUserCommand(email, password);
        });
    }
    public static Faker<LoginUserCommand> WithEmail(this Faker<LoginUserCommand> faker, string email)
    {
        return faker.RuleFor(c => c.Email, g => email);
    }
    public static Faker<LoginUserCommand> WithPassword(this Faker<LoginUserCommand> faker, string password)
    {
        return faker.RuleFor(c => c.Password, g => password);
    }
    public static Faker<LoginUserCommand> WithInvalidEmail(this Faker<LoginUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Email,
            g => g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName).Replace("@", "")
        );
    }
    public static Faker<LoginUserCommand> WithTooShortPassword(this Faker<LoginUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength - 1, regexPattern: "[A-Za-z0-9]", prefix: "Aa0")
        );
    }
    public static Faker<LoginUserCommand> WithPasswordWithoutUppercaseLetters(this Faker<LoginUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[a-z0-9]", prefix: "a0")
        );
    }
    public static Faker<LoginUserCommand> WithPasswordWithoutLowercaseLetters(this Faker<LoginUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Z0-9]", prefix: "A0")
        );
    }
    public static Faker<LoginUserCommand> WithPasswordWithoutDigits(this Faker<LoginUserCommand> faker)
    {
        return faker.RuleFor(
            c => c.Password,
            g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Za-z]", prefix: "Aa")
        );
    }
}
public static class LoginUserEndpoint
{
    public const string Url = "/auth/login";

    public static async Task<IResult> LoginUser(
        [FromBody] LoginUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<LoginUserResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static void AddLoginUserEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(Url, LoginUser)
            .WithName(nameof(LoginUser))
            .Produces<LoginUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
    }
    public static async ValueTask<HttpResponseMessage> SendLoginUserAsync(
        this HttpClient httpClient,
        LoginUserCommand command,
        CancellationToken cancellationToken
    )
    {
        return await HttpClientJsonExtensions.PostAsJsonAsync(httpClient, Url, command, cancellationToken);
    }
}