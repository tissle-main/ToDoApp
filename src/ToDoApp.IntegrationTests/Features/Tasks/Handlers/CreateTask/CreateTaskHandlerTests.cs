using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Web.Features.Tasks.Handlers.CreateTask;

namespace ToDoApp.IntegrationTests.Features.Tasks.Handlers.CreateTask;

public sealed class CreateTaskHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldCreateTask()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
        });
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        Guid response = await message.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            TaskEntity? entity = await db.Tasks.SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Id == response,
                TestContext.Current.CancellationToken
            );
            entity.Should().NotBeNull();
            entity.ToDto().Should().BeEquivalentTo(dto, cfg => cfg.Excluding(dto => dto.Id));
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldCreateJoinEntities()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        CategoryEntity category = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            CategoryEntity[] categories = await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return Faker.PickRandom(categories);
        });
        Task_Category_JoinEntity je = new()
        {
            RightId = category.Id,
            Right = category
        };
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().WithCategories([je]).Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        Guid response = await message.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            Task_Category_JoinEntity? entity = await db.Tasks_Categories.SingleOrDefaultAsync(
                je => je.LeftId == response && je.RightId == category.Id,
                TestContext.Current.CancellationToken
            );
            entity.Should().NotBeNull();
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        TaskDto dto = new Faker<TaskEntity>().ValidInstance().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateTaskAsync("", dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateTaskAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }
}