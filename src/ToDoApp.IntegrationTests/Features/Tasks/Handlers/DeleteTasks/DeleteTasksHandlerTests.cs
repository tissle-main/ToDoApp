using Bogus;
using AwesomeAssertions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Web.Features.Tasks.Handlers.DeleteTasks;

namespace ToDoApp.IntegrationTests.Features.Tasks.Handlers.DeleteTasks;

public sealed class DeleteTasksHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldDeleteAllTasks_WhenNoIdsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (TaskEntity[] tasks, int taskCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int taskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            TaskEntity[] tasks = await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return (tasks, taskCount);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteTasksAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyTasks = await db.Tasks.AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            anyTasks.Should().BeFalse();
            int newTaskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            newTaskCount.Should().Be(taskCount - tasks.Length);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldDeleteConcreteTasks_WhenIdsAreProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (TaskEntity[] tasks, int taskCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int taskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            TaskEntity[] tasks = await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return (tasks, taskCount);
        });
        Guid[] ids = Faker.PickRandom(tasks, Faker.Random.Number(1, tasks.Length)).Select(e => e.Id).ToArray();
        tasks = tasks.Where(e => !ids.Contains(e.Id)).ToArray();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteTasksAsync(accessToken, ids, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            TaskEntity[] newTasks = await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            newTasks.Should().BeEquivalentTo(tasks);
            int newTaskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            newTaskCount.Should().Be(taskCount - ids.Length);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldDeleteJoinEntities()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (TaskEntity[] tasks, int taskCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            int taskCount = await db.Tasks.CountAsync(TestContext.Current.CancellationToken);
            TaskEntity[] tasks = await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return (tasks, taskCount);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteTasksAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyJe = await db.Tasks_Categories.Include(je => je.Left).AnyAsync(je => je.Left!.UserId == user.Id, TestContext.Current.CancellationToken);
            anyJe.Should().BeFalse();
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteTasksAsync("", [], TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteTasksAsync(
            accessToken,
            [Guid.NewGuid()],
            TestContext.Current.CancellationToken
        );

        //Assert
        message.Should().Be404NotFound();
    }
}