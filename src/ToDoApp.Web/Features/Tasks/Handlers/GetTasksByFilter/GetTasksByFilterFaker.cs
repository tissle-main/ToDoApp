using Bogus;

namespace ToDoApp.Web.Features.Tasks.Handlers.GetTasksByFilter;

public static class GetTasksByFilterFaker
{
    extension(Faker<GetTasksByFilterQuery> thisFaker)
    {
        public Faker<GetTasksByFilterQuery> ValidInstance()
        {
            return thisFaker.CustomInstantiator(g =>
            {
                return new GetTasksByFilterQuery();
            });
        }
        public Faker<GetTasksByFilterQuery> WithSearch(string search)
        {
            return thisFaker.RuleFor(e => e.Search, g => search);
        }
        public Faker<GetTasksByFilterQuery> WithCategory(string category)
        {
            return thisFaker.RuleFor(e => e.Category, g => category);
        }
        public Faker<GetTasksByFilterQuery> WithDone(bool done)
        {
            return thisFaker.RuleFor(e => e.Done, g => done);
        }
        public Faker<GetTasksByFilterQuery> WithSkip(int skip)
        {
            return thisFaker.RuleFor(e => e.Skip, g => skip);
        }
        public Faker<GetTasksByFilterQuery> WithTake(int take)
        {
            return thisFaker.RuleFor(e => e.Take, g => take);
        }
        public Faker<GetTasksByFilterQuery> WithNegativeSkip()
        {
            return thisFaker.RuleFor(e => e.Skip, g => -1);
        }
        public Faker<GetTasksByFilterQuery> WithNegativeTake()
        {
            return thisFaker.RuleFor(e => e.Take, g => -1);
        }
    }
}