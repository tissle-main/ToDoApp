using Bogus;
using ToDoApp.Data.Features.Tasks;

namespace ToDoApp.Web.Shared.Fakers;

public static class TaskEntityFaker
{
    public static Faker<TaskEntity> ValidInstance(this Faker<TaskEntity> faker, Guid userId = default)
    {
        return faker.CustomInstantiator(g =>
        {
            return new TaskEntity()
            {
                Title = g.Random.String(TaskEntityConstants.TitleMaxLength),
                Description = g.Random.String(TaskEntityConstants.DescriptionMaxLength),
                Done = g.Random.Bool(),
                UserId = userId
            };
        });
    }
    public static Faker<TaskEntity> WithEmptyTitle(this Faker<TaskEntity> faker)
    {
        return faker.RuleFor(dto => dto.Title, g => string.Empty);
    }
    public static Faker<TaskEntity> WithTooLargeTitle(this Faker<TaskEntity> faker)
    {
        return faker.RuleFor(dto => dto.Title, g => g.Random.String(TaskEntityConstants.TitleMaxLength + 1));
    }
    public static Faker<TaskEntity> WithTooLargeDescription(this Faker<TaskEntity> faker)
    {
        return faker.RuleFor(dto => dto.Description, g => g.Random.String(TaskEntityConstants.DescriptionMaxLength + 1));
    }
}