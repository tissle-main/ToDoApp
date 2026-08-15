using Bogus;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Features.Categories;
using ToDoApp.IntegrationTests.Seeders;
using ToDoApp.Web.Features.Categories.Dtos;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Categories.Handlers.UpdateCategory;

namespace ToDoApp.IntegrationTests.Features.Categories;

public sealed class UpdateCategoryHandlerTests(ToDoAppFixture thisApp)
{
    private Faker Faker { get; } = new();

    [Fact]
    public async ValueTask Handler_ShouldUpdateCategory()
    {
        //Arrange
        await thisApp.ResetDatabaseAsync();
        (UserEntity user, _, string accessToken) = await thisApp.AddUsers2AndLoginRandom();
        CategoryDto dto = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Dictionary<Guid, List<CategoryEntity>> dict = await new Faker<CategoryEntity>()
                .ValidInstance(default, default)
                .SeedDatabaseForAllUsers(db, TestContext.Current.CancellationToken);
            return Faker.PickRandom(dict[user.Id]).ToDto();
        });
        new Faker<CategoryEntity>().ValidInstance(dto.Id, user.Id).Generate().MapToDto(dto);

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be204NoContent();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            CategoryEntity? entity = await db.Categories.AsNoTracking().SingleOrDefaultAsync(
                e => e.UserId == user.Id && e.Id == dto.Id,
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
        using HttpResponseMessage response = await thisApp.HttpClient.SendUpdateCategoryAsync("", dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage response = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

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
        using HttpResponseMessage response = await thisApp.HttpClient.SendUpdateCategoryAsync(accessToken, dto, TestContext.Current.CancellationToken);

        //Assert
        response.Should().Be422UnprocessableEntity();
    }
}