using Bogus;

namespace ToDoApp.Web.Features.Auth.Handlers.RegisterUser;

public static class RegisterUserCommandFaker
{
    extension(Faker<RegisterUserCommand> thisFaker)
    {
        public Faker<RegisterUserCommand> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                string email = g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName);
                string password = g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Za-z0-9]", prefix: "Aa0");
                return new RegisterUserCommand(email, password);
            });
        }
        public Faker<RegisterUserCommand> WithEmail(string email)
        {
            return thisFaker.RuleFor(c => c.Email, g => email);
        }
        public Faker<RegisterUserCommand> WithPassword(string password)
        {
            return thisFaker.RuleFor(c => c.Password, g => password);
        }
        public Faker<RegisterUserCommand> WithInvalidEmail()
        {
            return thisFaker.RuleFor(
                c => c.Email,
                g => g.Internet.ExampleEmail(g.Person.FirstName, g.Person.LastName).Replace("@", "")
            );
        }
        public Faker<RegisterUserCommand> WithTooShortPassword()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstants.PasswordMinLength - 1, regexPattern: "[A-Za-z0-9]", prefix: "Aa0")
            );
        }
        public Faker<RegisterUserCommand> WithPasswordWithoutUppercaseLetters()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[a-z0-9]", prefix: "a0")
            );
        }
        public Faker<RegisterUserCommand> WithPasswordWithoutLowercaseLetters()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Z0-9]", prefix: "A0")
            );
        }
        public Faker<RegisterUserCommand> WithPasswordWithoutDigits()
        {
            return thisFaker.RuleFor(
                c => c.Password,
                g => g.Internet.Password(length: AuthConstants.PasswordMinLength, regexPattern: "[A-Za-z]", prefix: "Aa")
            );
        }
    } 
}