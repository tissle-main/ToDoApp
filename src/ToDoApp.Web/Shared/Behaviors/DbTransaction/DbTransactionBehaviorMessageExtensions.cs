using System.Runtime.CompilerServices;

namespace ToDoApp.Web.Shared.Behaviors.DbTransaction;

public static class DbTransactionBehaviorMessageExtensions
{
    private static ConditionalWeakTable<IDbTransactionBehaviorMessage, DbTransactionBehaviorMessageExtraProperties> ExtraPropertiesTable { get; } = [];

    extension(IDbTransactionBehaviorMessage thisMessage)
    {
        public bool BeginDbTransaction
        {
            get => thisMessage.GetExtraProperties().BeginDbTransaction;
            set
            {
                thisMessage.GetExtraProperties().BeginDbTransaction = value;
            }
        }

        private DbTransactionBehaviorMessageExtraProperties GetExtraProperties()
        {
            return ExtraPropertiesTable.GetOrAdd(thisMessage, static _ => new DbTransactionBehaviorMessageExtraProperties());
        }
    }
}