using Bogus;
using ToDoApp.Web.Features.Auth;
using ToDoApp.Web.Features.Auth.Handlers;

namespace ToDoApp.Web.Shared.Fakers;

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