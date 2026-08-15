using Microsoft.EntityFrameworkCore.Storage;

namespace ToDoApp.Web.Shared.Behaviors.DbTransaction;

public sealed class DbTransactionBehaviorMessageExtraProperties
{
    public bool BeginDbTransaction { get; set; } = true;
}