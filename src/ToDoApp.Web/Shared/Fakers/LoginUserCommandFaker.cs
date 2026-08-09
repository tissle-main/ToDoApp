using Bogus;
using ToDoApp.Web.Features.Auth;
using ToDoApp.Web.Features.Auth.Handlers;

namespace ToDoApp.Web.Shared.Fakers;

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