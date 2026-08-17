using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Extensions;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

namespace ToDoApp.IntegrationTests.Features.Tasks.Handlers.GetTasksByFilter;

public sealed class GetTasksByFilterHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldReturnAllTasks_WhenNoFilterIsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
        });
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteTasks_WhenSearchIsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
        });
        TaskEntity task = Faker.PickRandom(tasks);
        string search = Faker.Random.Substring(task.Title);
        tasks = tasks.Where(t => t.Title.Contains(search) || t.Description?.Contains(search) is true).ToArray();

        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithSearch(search).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteTasks_WhenCategoryIsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            return await db.Tasks.Include(e => e.Categories).ThenInclude(je => je.Right).Where(e => e.UserId == user.Id).ToArrayAsync(
                TestContext.Current.CancellationToken
            );
        });
        TaskEntity task = Faker.PickRandom(tasks);
        string[] categories = task.Categories.Select(je => je.Right!.Name).ToArray();
        string category = Faker.PickRandom(categories);
        tasks = tasks.Where(t => t.Categories.Select(je => je.Right!.Name).Contains(category)).ToArray();

        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithCategory(category).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteTasks_WhenDoneIsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(
                TestContext.Current.CancellationToken
            );
        });
        TaskEntity task = Faker.PickRandom(tasks);
        tasks = tasks.Where(t => t.Done == task.Done).ToArray();

        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithDone(task.Done).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteTasks_WhenSkipIsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(
                TestContext.Current.CancellationToken
            );
        });
        int skip = Faker.Random.Number(1, tasks.Length - 1);
        tasks = tasks.Skip(skip).ToArray();

        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithSkip(skip).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteTasks_WhenTakeIsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity[] tasks = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(
                TestContext.Current.CancellationToken
            );
        });
        int take = Faker.Random.Number(1, tasks.Length - 1);
        tasks = tasks.Take(take).ToArray();

        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithTake(take).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        TaskDto[]? response = await message.Content.ReadFromJsonAsync<TaskDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(tasks.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync("", query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be401Unauthorized();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenSkipIsNegative()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNegativeSkip().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenTakeIsNegative()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        GetTasksByFilterQuery query = new Faker<GetTasksByFilterQuery>().ValidInstance().WithNegativeTake().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetTasksByFilterAsync(accessToken, query, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }
}