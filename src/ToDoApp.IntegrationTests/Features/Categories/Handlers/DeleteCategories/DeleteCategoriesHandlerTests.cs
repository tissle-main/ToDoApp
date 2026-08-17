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
using ToDoApp.Web.Features.Categories.Handlers.DeleteCategories;

namespace ToDoApp.IntegrationTests.Features.Categories.Handlers.DeleteCategories;

public sealed class DeleteCategoriesHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldDeleteAllCategories_WhenNoIdsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (CategoryEntity[] categories, int categoriesCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int categoriesCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            CategoryEntity[] categories = await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken); 
            return (categories, categoriesCount);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteCategoriesAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyCategories = await db.Categories.AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            anyCategories.Should().BeFalse();
            int newCategoriesCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            newCategoriesCount.Should().Be(categoriesCount - categories.Length);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldDeleteConcreteCategories_WhenIdsAreProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (CategoryEntity[] categories, int categoriesCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await new Faker<TaskEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            await db.Seed_Task_Category_JoinEntitiesForAllUsersAsync(TestContext.Current.CancellationToken);
            int categoriesCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            CategoryEntity[] categories = await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return (categories, categoriesCount);
        });
        Guid[] ids = Faker.PickRandom(categories, Faker.Random.Number(1, categories.Length)).Select(e => e.Id).ToArray();
        categories = categories.Where(e => !ids.Contains(e.Id)).ToArray();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteCategoriesAsync(accessToken, ids, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity[] newCategories = await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            newCategories.Should().BeEquivalentTo(categories);
            int newCategoriesCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            newCategoriesCount.Should().Be(categoriesCount - ids.Length);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldDeleteJoinEntities()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        (CategoryEntity[] categories, int categoriesCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            int categoriesCount = await db.Categories.CountAsync(TestContext.Current.CancellationToken);
            CategoryEntity[] categories = await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            return (categories, categoriesCount);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteCategoriesAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyJe = await db.Tasks_Categories.Include(je => je.Right).AnyAsync(je => je.Right!.UserId == user.Id, TestContext.Current.CancellationToken);
            anyJe.Should().BeFalse();
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteCategoriesAsync("", [], TestContext.Current.CancellationToken);

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
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendDeleteCategoriesAsync(
            accessToken,
            [Guid.NewGuid()],
            TestContext.Current.CancellationToken
        );

        //Assert
        message.Should().Be404NotFound();
    }
}