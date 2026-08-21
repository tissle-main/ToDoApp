using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ToDoApp.Data.Shared.CreationTimeEntities;

public static class CreationTimeEntityDbContextExtensions
{
    extension(AppDbContext thisDbContext)
    {
        public void SetUtcNowForCreationTimeEntities()
        {
            foreach(EntityEntry<ICreationTimeEntity> entry in thisDbContext.ChangeTracker.Entries<ICreationTimeEntity>())
            {
                if(entry.State is EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}