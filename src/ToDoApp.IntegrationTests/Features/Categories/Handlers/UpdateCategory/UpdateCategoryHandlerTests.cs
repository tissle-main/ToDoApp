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
using ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;

namespace ToDoApp.IntegrationTests.Features.Categories.Handlers.UpdateCategory;

public sealed class UpdateCategoryHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldUpdateCategory()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        CategoryDto dto = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            CategoryEntity[] categories = await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return Faker.PickRandom(categories).ToDto();
        });
        new Faker<CategoryEntity>().ValidInstance().WithId(dto.Id).WithUserId(user.Id).Generate().MapToDto(dto);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity? entity = await db.Categories.SingleOrDefaultAsync(
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
        CategoryEntity category = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            CategoryEntity[] categories = await db.Categories.Include(c => c.Tasks).Where(
                e => e.UserId == user.Id
            ).ToArrayAsync(TestContext.Current.CancellationToken);
            return Faker.PickRandom(categories);
        });
        Task_Category_JoinEntity je = Faker.PickRandom(category.Tasks);
        new Faker<CategoryEntity>().ValidInstance().WithId(category.Id).WithUserId(user.Id).WithTasks([je]).Generate().MapToEntity(category);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, category.ToDto(), TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity? entity = await db.Categories.Include(e => e.Tasks).SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Id == category.Id,
                TestContext.Current.CancellationToken
            );
            entity.Should().NotBeNull();
            Task_Category_JoinEntity? actualJe = entity.Tasks.SingleOrDefault();
            actualJe.Should().NotBeNull();
            actualJe.Should().BeEquivalentTo(je);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance().Generate().ToDto();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateCategoryAsync("", dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be422UnprocessableEntity();
    }
}