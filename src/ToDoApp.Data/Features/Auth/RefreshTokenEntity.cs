using ToDoApp.Data.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data.Features.Auth;

public sealed class RefreshTokenEntity : IKeyedEntity
{
    //Value properties
    public Guid Id { get; set; }
    public required string Value { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
}
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
public static class RefreshTokenEntityConstants
{
    public const int RefreshTokenMaxLength = 128;
}