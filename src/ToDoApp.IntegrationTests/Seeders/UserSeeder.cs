using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Web.Shared.Fakers;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Auth.Handlers;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Handlers.RegisterUser;
using ToDoApp.Web.Features.Auth.Handlers.LoginUser;

namespace ToDoApp.IntegrationTests.Seeders;

public static class UserSeeder
{
    private static Faker Faker { get; } = new();

    public static async ValueTask<List<RegisterUserCommand>> AddUsers(this ToDoAppFixture app)
    {
        List<RegisterUserCommand> users = [];
        for(int count = Faker.Random.Number(2, 5); count > 0; count--)
        {
            RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance();
            using HttpResponseMessage message = await app.HttpClient.SendRegisterUserAsync(command, TestContext.Current.CancellationToken);
            message.EnsureSuccessStatusCode();
            users.Add(command);
        }
        return users;
    }
    public static async ValueTask<IEnumerable<(UserEntity User, string Password)>> AddUsers2(this ToDoAppFixture app)
    {
        List<RegisterUserCommand> commands = await app.AddUsers();
        return await app.ExecuteDbContextAsync(async db =>
        {
            UserEntity[] users = await db.Users.AsNoTracking().Where(
                e => commands.Select(c => c.Email).Contains(e.Email)
            ).ToArrayAsync(TestContext.Current.CancellationToken);
            return users.Select(e =>
            {
                string password = commands.First(c => c.Email == e.Email).Password;
                return (e, password);
            });
        });
    }
    public static async ValueTask<RegisterUserCommand> AddUsersAndPickRandom(this ToDoAppFixture app)
    {
        return Faker.PickRandom(await app.AddUsers());
    }
    public static async ValueTask<(UserEntity User, string Password)> AddUsers2AndPickRandom(this ToDoAppFixture app)
    {
        return Faker.PickRandom(await app.AddUsers2());
    }
    public static async ValueTask<(UserEntity User, string Password, string AccessToken)> AddUsers2AndLoginRandom(this ToDoAppFixture app)
    {
        (UserEntity user, string password) = await app.AddUsers2AndPickRandom();
        LoginUserCommand login = new(user.Email!, password);
        using HttpResponseMessage response = await app.HttpClient.SendLoginUserAsync(login, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        LoginUserResponse? result = await response.Content.ReadFromJsonAsync<LoginUserResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        return (user, password, result.AccessToken);
    }
}