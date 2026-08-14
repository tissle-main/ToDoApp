using Microsoft.EntityFrameworkCore;
using ToDoApp.Data.Shared.KeyedEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Auth.RefreshTokens;

public sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    #region Interfaces
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ConfigureKeyedEntity();
        builder.Property(e => e.Value).IsRequired().HasMaxLength(RefreshTokenEntityConstants.RefreshTokenMaxLength);
        builder.HasIndex(e => e.Value).IsUnique();
        builder.Property(e => e.ExpiresAt).IsRequired();
        builder.HasOne(e => e.User).WithMany(e => e.RefreshTokens).IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
    #endregion
}