using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Categories.Handlers.GetCategories;

namespace ToDoApp.IntegrationTests.Features.Categories;

public sealed class GetCategoriesHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldReturnAllCategories_WhenNoIdsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        List<CategoryEntity> categories = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Dictionary<Guid, List<CategoryEntity>> dict = await new Faker<CategoryEntity>()
                .ValidInstance(default, default)
                .SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            return dict[user.Id];
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendGetCategoriesAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        GetCategoriesResponse? result = await response.Content.ReadFromJsonAsync<GetCategoriesResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Categories.Should().BeEquivalentTo(categories.ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteCategories_WhenIdsAreProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        List<CategoryEntity> categories = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Dictionary<Guid, List<CategoryEntity>> dict = await new Faker<CategoryEntity>()
                .ValidInstance(default, default)
                .SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            return dict[user.Id];
        });
        Guid[] ids = Faker.PickRandom(categories, Faker.Random.Number(1, categories.Count)).Select(e => e.Id).ToArray();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendGetCategoriesAsync(accessToken, ids, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        GetCategoriesResponse? result = await response.Content.ReadFromJsonAsync<GetCategoriesResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        result.Categories.Should().BeEquivalentTo(categories.Where(e => ids.Contains(e.Id)).ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendGetCategoriesAsync("", [], TestContext.Current.CancellationToken);

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
        using HttpResponseMessage response = await thisApp.HttpClient.SendGetCategoriesAsync(
            accessToken,
            [Guid.NewGuid()],
            TestContext.Current.CancellationToken
        );

        //Assert
        response.Should().Be404NotFound();
    }
}