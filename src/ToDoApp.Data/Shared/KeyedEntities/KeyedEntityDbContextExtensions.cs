using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ToDoApp.Data.Shared.KeyedEntities;

public static class KeyedEntityDbContextExtensions
{
    public static void GenerateIdForKeyedEntities(this AppDbContext dbContext)
    {
        foreach(EntityEntry<IKeyedEntity> entry in dbContext.ChangeTracker.Entries<IKeyedEntity>())
        {
            if(entry.State is EntityState.Added)
            {
                entry.Entity.Id = Guid.CreateVersion7();
            }
        }
    }
}