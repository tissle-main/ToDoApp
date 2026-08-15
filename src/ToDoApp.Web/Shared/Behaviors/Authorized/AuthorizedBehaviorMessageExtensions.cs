using System.Runtime.CompilerServices;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Shared.Behaviors.Authorized;

public static class AuthorizedBehaviorMessageExtensions
{
    private static ConditionalWeakTable<IAuthorizedBehaviorMessage, AuthorizedBehaviorMessageExtraProperties> ExtraPropertiesTable { get; } = [];

    extension(IAuthorizedBehaviorMessage thisMessage)
    {
        public UserEntity User
        {
            get => thisMessage.GetExtraProperties().User;
            set
            {
                thisMessage.GetExtraProperties().User = value;
            }
        }

        private AuthorizedBehaviorMessageExtraProperties GetExtraProperties()
        {
            return ExtraPropertiesTable.GetOrAdd(thisMessage, static _ => new AuthorizedBehaviorMessageExtraProperties());
        }
    }
}