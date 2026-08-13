using Bogus;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Web.Features.Tasks.Handlers;

namespace ToDoApp.Web.Shared.Fakers;

public static class GetTasksByFilterQueryFaker
{
    public static Faker<GetTasksByFilterQuery> ValidInstance(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.CustomInstantiator(g =>
        {
            string search = g.Random.String(TaskEntityConstants.DescriptionMaxLength);
            string category = g.Random.String(CategoryEntityConstants.NameMaxLength);
            bool done = g.Random.Bool();
            int skip = g.Random.Number(0, 10);
            int take = g.Random.Number(0, 10);
            return new GetTasksByFilterQuery(search, category, done, skip, take);
        });
    }
    public static Faker<GetTasksByFilterQuery> WithNoSkip(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.RuleFor(e => e.Skip, g => null);
    }
    public static Faker<GetTasksByFilterQuery> WithNegativeSkip(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.RuleFor(e => e.Skip, g => -1);
    }
    public static Faker<GetTasksByFilterQuery> WithNoTake(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.RuleFor(e => e.Take, g => null);
    }
    public static Faker<GetTasksByFilterQuery> WithNegativeTake(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.RuleFor(e => e.Take, g => -1);
    }
    public static Faker<GetTasksByFilterQuery> WithTooLargeSearch(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.RuleFor(e => e.Search, g => g.Random.String(TaskEntityConstants.DescriptionMaxLength + 1));
    }
    public static Faker<GetTasksByFilterQuery> WithTooLargeCategory(this Faker<GetTasksByFilterQuery> faker)
    {
        return faker.RuleFor(e => e.Category, g => g.Random.String(CategoryEntityConstants.NameMaxLength + 1));
    }
}