using EntityFrameworkCore.Triggered.Extensions;
using EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity.Models;

namespace EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity.Triggers.Users
{
    public class UpdateUserFullName : Trigger<User>
    {
        public UpdateUserFullName() { }

        public override void BeforeSave(ITriggerContext<User> context)
        {
            var user = context.Entity;
            user.FullName = $"{user.Name.FirstName} {user.Name.LastName}";
        }
    }
}
