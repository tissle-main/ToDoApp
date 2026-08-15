using Bogus;
using AwesomeAssertions;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Categories.Handlers.CreateCategory;

namespace ToDoApp.IntegrationTests.Features.Categories;

public sealed class CreateCategoryHandlerTests(ToDoAppFixture thisApp)
{
    [Fact]
    public async ValueTask Handler_ShouldCreateCategory()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<CategoryEntity>().ValidInstance(default, default).SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
        });
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default, default).Generate().ToDto();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be200Ok();
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.Should().Be("");
        CreateCategoryResponse? result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>(TestContext.Current.CancellationToken);
        result.Should().NotBeNull();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity? entity = await db.Categories.AsNoTracking().SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Id == result.CreatedId,
                TestContext.Current.CancellationToken
            );
            entity.Should().NotBeNull();
            entity.ToDto().Should().BeEquivalentTo(dto);
        });
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenUnauthorized()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default, default).Generate().ToDto();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendCreateCategoryAsync("", dto, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be401Unauthorized();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoNameIsEmpty()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default, default).WithEmptyName().Generate().ToDto();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }

    [Fact]
    public async ValueTask Handler_ShouldFail_WhenDtoNameIsTooLarge()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (_, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        CategoryDto dto = new Faker<CategoryEntity>().ValidInstance(default, default).WithTooLargeName().Generate().ToDto();

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendCreateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }
}