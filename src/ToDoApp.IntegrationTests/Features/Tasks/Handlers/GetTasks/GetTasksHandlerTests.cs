using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks.Handlers.GetTasks;

namespace ToDoApp.IntegrationTests.Features.Tasks.Handlers.GetTasks;

public sealed class GetTasksHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldReturnAllTasks_WhenNoIdsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.OrderByDescending(e => e.CreatedAt).ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteTasks_WhenIdsAreProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Guid[] ids = Faker.PickRandom(tasks, Faker.Random.Number(1, tasks.Length)).Select(e => e.Id).ToArray();
        tasks = tasks.Where(task => ids.Contains(task.Id)).ToArray();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksAsync(accessToken, ids, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.OrderByDescending(e => e.CreatedAt).ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksAsync("", [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be401Unauthorized();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenSomeOfProvidedIdsNotExist()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksAsync(
            accessToken,
            [Guid.NewGuid()],
            TestContext.Current.CancellationToken
        );

        //Assert
        message.Should().Be404NotFound();
    }
}