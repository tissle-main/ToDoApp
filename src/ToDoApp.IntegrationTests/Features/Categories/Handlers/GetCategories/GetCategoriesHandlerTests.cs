using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Web.Features.Categories.Handlers.GetCategories;

namespace ToDoApp.IntegrationTests.Features.Categories.Handlers.GetCategories;

public sealed class GetCategoriesHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldReturnAllCategories_WhenNoIdsProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        CategoryEntity[] categories = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
        });

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetCategoriesAsync(accessToken, [], TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        CategoryDto[]? response = await message.Content.ReadFromJsonAsync<CategoryDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(categories.OrderBy(e => e.CreatedAt).ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldReturnConcreteCategories_WhenIdsAreProvided()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandomAsync();
        CategoryEntity[] categories = await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance().SeedDatabaseForAllUsersAsync(db, TestContext.Current.CancellationToken);
            return await db.Categories.Where(e => e.UserId == user.Id).ToArrayAsync(TestContext.Current.CancellationToken);
        });
        Guid[] ids = Faker.PickRandom(categories, Faker.Random.Number(1, categories.Length)).Select(e => e.Id).ToArray();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetCategoriesAsync(accessToken, ids, TestContext.Current.CancellationToken);

        //Assert
        message.Should().Be200Ok();
        CategoryDto[]? response = await message.Content.ReadFromJsonAsync<CategoryDto[]>(TestContext.Current.CancellationToken);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(categories.Where(e => ids.Contains(e.Id)).OrderBy(e => e.CreatedAt).ToDtos());
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetCategoriesAsync("", [], TestContext.Current.CancellationToken);

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
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetCategoriesAsync(
            accessToken,
            [Guid.NewGuid()],
            TestContext.Current.CancellationToken
        );

        //Assert
        message.Should().Be404NotFound();
    }
}