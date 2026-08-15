using Bogus;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Categories.Dtos;

public static class CategoryEntityFaker
{
    extension(Faker<CategoryEntity> thisFaker)
    {
        public Faker<CategoryEntity> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                return new CategoryEntity()
                {
                    Name = g.Random.String2(CategoryEntityConstants.NameMaxLength)
                };
            });
        }
        public Faker<CategoryEntity> WithId(Guid id)
        {
            return thisFaker.RuleFor(dto => dto.Id, g => id);
        }
        public Faker<CategoryEntity> WithUserId(Guid userId)
        {
            return thisFaker.RuleFor(dto => dto.UserId, g => userId);
        }
        public Faker<CategoryEntity> WithTasks(List<Task_Category_JoinEntity> tasks)
        {
            return thisFaker.RuleFor(e => e.Tasks, g => tasks);
        }
        public Faker<CategoryEntity> WithEmptyName()
        {
            return thisFaker.RuleFor(dto => dto.Name, g => string.Empty);
        }
        public Faker<CategoryEntity> WithTooLargeName()
        {
            return thisFaker.RuleFor(
                dto => dto.Name,
                g => g.Random.String2(CategoryEntityConstants.NameMaxLength + 1)
            );
        }
    }
}