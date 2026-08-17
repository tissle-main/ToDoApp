using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Data.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Web.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Tasks_Categories;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

namespace ToDoApp.IntegrationTests.Features.Categories.Handlers.CreateCategory;

public sealed class CreateCategoryHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldCreateCategory()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
        });
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        Guid response = await message.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity? entity = await db.Categories.SingleOrDefaultAsync(
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
        TaskEntity task = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            TaskEntity[] tasks = await db.Tasks.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return Faker.PickRandom(tasks);
        });
        Task_Category_JoinEntity je = new()
        {
            LeftId = task.Id
        };
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().WithTasks([je]).Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        Guid response = await message.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            Task_Category_JoinEntity? entity = await db.Tasks_Categories.SingleOrDefaultAsync(
                je => je.LeftId == task.Id && je.RightId == response,
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
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateCategoryAsync("", dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be401Unauthorized();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoNameIsEmpty()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().WithEmptyName().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoNameIsTooLarge()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().WithTooLargeName().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }
}