using Bogus;
using ToDoApp.Data.Features.Categories;

namespace ToDoApp.Web.Shared.Fakers;

public static class CategoryEntityFaker
{
    public static Faker<CategoryEntity> ValidInstance(this Faker<CategoryEntity> faker, Guid userId = default)
    {
        return faker.CustomInstantiator(g =>
        {
            return new CategoryEntity()
            {
                Name = g.Random.String(CategoryEntityConstants.NameMaxLength),
                UserId = userId
            };
        });
    }
    public static Faker<CategoryEntity> WithEmptyName(this Faker<CategoryEntity> faker)
    {
        return faker.RuleFor(dto => dto.Name, g => string.Empty);
    }
    public static Faker<CategoryEntity> WithTooLargeName(this Faker<CategoryEntity> faker)
    {
        return faker.RuleFor(dto => dto.Name, g => g.Random.String(CategoryEntityConstants.NameMaxLength + 1));
    }
}