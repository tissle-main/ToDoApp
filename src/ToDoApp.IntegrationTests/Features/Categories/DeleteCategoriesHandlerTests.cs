using Bogus;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Categories.Handlers.DeleteCategories;

namespace ToDoApp.IntegrationTests.Features.Categories;

public sealed class DeleteCategoriesHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldDeleteAllCategories_WhenNoIdsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        (List<CategoryEntity> categories, int categoriesCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Dictionary<Guid, List<CategoryEntity>> dict = await new Faker<CategoryEntity>()
                .ValidInstance(default, default)
                .SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            int categoriesCount = dict.Sum(list => list.Value.Count);
            return (dict[user.Id], categoriesCount);
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendDeleteCategoriesAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            bool anyCategories = await db.Categories.AsNoTracking().AnyAsync(e => e.UserId == user.Id, TestContext.Current.CancellationToken);
            anyCategories.Should().BeFalse();
            int newCategoriesCount = await db.Categories.AsNoTracking().CountAsync(TestContext.Current.CancellationToken);
            newCategoriesCount.Should().Be(categoriesCount - categories.Count);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldDeleteConcreteCategories_WhenIdsAreProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        (List<CategoryEntity> categories, int categoriesCount) = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Dictionary<Guid, List<CategoryEntity>> dict = await new Faker<CategoryEntity>()
                .ValidInstance(default, default)
                .SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            int categoriesCount = dict.Sum(list => list.Value.Count);
            return (dict[user.Id], categoriesCount);
        });
        Guid[] ids = Faker.PickRandom(categories, Faker.Random.Number(1, categories.Count)).Select(e => e.Id).ToArray();
        categories = categories.Where(e => !ids.Contains(e.Id)).ToList();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendDeleteCategoriesAsync(accessToken, ids, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity[] newCategories = await db.Categories.AsNoTracking().Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
            newCategories.Should().BeEquivalentTo(categories);
            int newCategoriesCount = await db.Categories.AsNoTracking().CountAsync(TestContext.Current.CancellationToken);
            newCategoriesCount.Should().Be(categoriesCount - ids.Length);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendDeleteCategoriesAsync("", [], TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be401Unauthorized();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenSomeOfProvidedIdsNotExist()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance(default, default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendDeleteCategoriesAsync(
            accessToken,
            [Guid.NewGuid()],
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be404NotFound();
    }
}