namespace ToDoApp.Web.Shared.JoinEntities;

public static class UpdateJoinEntitiesEndpoint
{
    extension(RouteHandlerBuilder thisBuilder)
    {
        public RouteHandlerBuilder AddUpdateJoinEntitiesProductionProblems()
        {
            thisBuilder.ProducesProblem(StatusCodes.Status401Unauthorized);
            thisBuilder.ProducesProblem(StatusCodes.Status403Forbidden);
            return thisBuilder.ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}