using Bogus;
using AwesomeAssertions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.IntegrationTests.Features.Task_Categories;

public sealed class Task_Category_UpdateHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldRemoveJoinEntities()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        Task_Category_JoinEntity[] jes = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            return await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Task_Category_JoinEntity[] expectedJes = Faker.PickRandom(jes, Faker.Random.Number(0, jes.Length - 1)).ToArray();
        Task_Category_UpdateCommand command = new(jes, expectedJes);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            Task_Category_JoinEntity[] newJes = await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
            newJes.Should().BeEquivalentTo(expectedJes);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldAddJoinEntities()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        Task_Category_JoinEntity[] jes = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            return await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Task_Category_JoinEntity[] existingJes = Faker.PickRandom(jes, Faker.Random.Number(0, jes.Length - 1)).ToArray();
        Task_Category_UpdateCommand command = new(jes, existingJes);
        using HttpResponseMessage preMessage = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);
        preMessage.Should().Be204NoContent();
        command = new Task_Category_UpdateCommand(existingJes, jes);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            Task_Category_JoinEntity[] newJes = await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
            newJes.Should().BeEquivalentTo(jes);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenOldEntitiesIsInvalid()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        Task_Category_JoinEntity[] jes = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            return await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Task_Category_JoinEntity[] expectedJes = Faker.PickRandom(jes, Faker.Random.Number(0, jes.Length - 1)).ToArray();
        Task_Category_UpdateCommand command = new([new Task_Category_JoinEntity()
        {
            LeftId = Guid.CreateVersion7(),
            RightId = Guid.CreateVersion7()
        }], expectedJes);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be500InternalServerError();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenNewEntitiesLeftIdsNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        Task_Category_JoinEntity[] jes = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            return await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Task_Category_JoinEntity[] expectedJes = Faker.PickRandom(jes, Faker.Random.Number(1, jes.Length - 1)).Select(je => new Task_Category_JoinEntity()
        {
            LeftId = Guid.CreateVersion7(),
            RightId = je.RightId
        }).ToArray();
        Task_Category_UpdateCommand command = new(jes, expectedJes);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be404NotFound();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenNewEntitiesRightIdsNotFound()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        Task_Category_JoinEntity[] jes = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            return await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Task_Category_JoinEntity[] expectedJes = Faker.PickRandom(jes, Faker.Random.Number(1, jes.Length - 1)).Select(je => new Task_Category_JoinEntity()
        {
            LeftId = je.LeftId,
            RightId = Guid.CreateVersion7()
        }).ToArray();
        Task_Category_UpdateCommand command = new(jes, expectedJes);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be404NotFound();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenNewEntitiesLeftHaveInvalidUser()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (Task_Category_JoinEntity[] userJes, Task_Category_JoinEntity[] notUserJes) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            Task_Category_JoinEntity[] userJes = await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);

            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken, [user.Id]);
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken, [user.Id]);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken, [user.Id]);
            Task_Category_JoinEntity[] notUserJes = await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
            notUserJes = notUserJes.Except(userJes).ToArray();
            return (userJes, notUserJes);
        });
        Task_Category_JoinEntity userJe = Faker.PickRandom(userJes);
        Task_Category_JoinEntity notUserJe = Faker.PickRandom(notUserJes);
        Task_Category_UpdateCommand command = new(userJes, [new Task_Category_JoinEntity()
        {
            LeftId = notUserJe.LeftId,
            RightId = userJe.RightId
        }]);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be403Forbidden();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenNewEntitiesRightHaveInvalidUser()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (Task_Category_JoinEntity[] userJes, Task_Category_JoinEntity[] notUserJes) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().WithUserId(user.Id).SeedDatabaseAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesAsync(user.Id, TestContext.Current.CancellationToken);
            Task_Category_JoinEntity[] userJes = await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);

            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken, [user.Id]);
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken, [user.Id]);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken, [user.Id]);
            Task_Category_JoinEntity[] notUserJes = await db.Tasks_Categories.ToArrayAsync(TestContext.Current.CancellationToken);
            notUserJes = notUserJes.Except(userJes).ToArray();
            return (userJes, notUserJes);
        });
        Task_Category_JoinEntity userJe = Faker.PickRandom(userJes);
        Task_Category_JoinEntity notUserJe = Faker.PickRandom(notUserJes);
        Task_Category_UpdateCommand command = new(userJes, [new Task_Category_JoinEntity()
        {
            LeftId = userJe.LeftId,
            RightId = notUserJe.RightId
        }]);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendTask_Category_UpdateAsync(accessToken, command, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be403Forbidden();
    }
}