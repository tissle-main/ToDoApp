using Bogus;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Tasks.Dtos;

public static class TaskEntityFaker
{
    extension(Faker<TaskEntity> thisFaker)
    {
        public Faker<TaskEntity> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                return new TaskEntity()
                {
                    Title = g.Random.String2(TaskEntityConstants.TitleMaxLength),
                    Description = g.Random.String2(TaskEntityConstants.DescriptionMaxLength),
                    Done = g.Random.Bool()
                };
            });
        }
        public Faker<TaskEntity> WithId(Guid id)
        {
            return thisFaker.RuleFor(e => e.Id, g => id);
        }
        public Faker<TaskEntity> WithUserId(Guid userId)
        {
            return thisFaker.RuleFor(e => e.UserId, g => userId);
        }
        public Faker<TaskEntity> WithCategories(List<Task_Category_JoinEntity> categories)
        {
            return thisFaker.RuleFor(e => e.Categories, g => categories);
        }
        public Faker<TaskEntity> WithEmptyTitle()
        {
            return thisFaker.RuleFor(e => e.Title, g => string.Empty);
        }
        public Faker<TaskEntity> WithTooLargeTitle()
        {
            return thisFaker.RuleFor(
                e => e.Title,
                g => g.Random.String2(TaskEntityConstants.TitleMaxLength + 1)
            );
        }
        public Faker<TaskEntity> WithTooLargeDescription()
        {
            return thisFaker.RuleFor(
                e => e.Description,
                g => g.Random.String2(TaskEntityConstants.DescriptionMaxLength + 1)
            );
        }
    }
}