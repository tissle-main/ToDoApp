using Bogus;
using AwesomeAssertions;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;

namespace ToDoApp.IntegrationTests.Features.Tasks.Handlers.UpdateTask;

public sealed class UpdateTaskHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldUpdateTask()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskDto dto = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            TaskEntity[] tasks = await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return Faker.PickRandom(tasks).ToDto();
        });
        new Faker<TaskEntity>().ValidInstance().WithId(dto.Id).WithUserId(user.Id).Generate().MapToDto(dto);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            TaskEntity? entity = await db.Tasks.SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Id == dto.Id,
                TestContext.Current.CancellationToken
            );
            entity.Should().NotBeNull();
            entity.ToDto().Should().BeEquivalentTo(dto);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldUpdateJoinEntities()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskEntity task = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            TaskEntity[] tasks = await db.Tasks.Include(c => c.Categories).Where(
                e => e.UserId == user.Id
            ).ToArrayAsync(TestContext.Current.CancellationToken);
            return Faker.PickRandom(tasks);
        });
        Task_Category_JoinEntity je = Faker.PickRandom(task.Categories);
        new Faker<TaskEntity>().ValidInstance().WithId(task.Id).WithUserId(user.Id).WithCategories([je]).Generate().MapToEntity(task);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateTaskAsync(accessToken, task.ToDto(), TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            TaskEntity? entity = await db.Tasks.Include(e => e.Categories).SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Id == task.Id,
                TestContext.Current.CancellationToken
            );
            entity.Should().NotBeNull();
            Task_Category_JoinEntity? actualJe = entity.Categories.SingleOrDefault();
            actualJe.Should().NotBeNull();
            actualJe.Should().BeEquivalentTo(je);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateTaskAsync("", dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be401Unauthorized();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoTitleIsEmpty()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithEmptyTitle().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoTitleIsTooLarge()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithTooLargeTitle().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoDescriptionIsTooLarge()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithTooLargeDescription().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }
}