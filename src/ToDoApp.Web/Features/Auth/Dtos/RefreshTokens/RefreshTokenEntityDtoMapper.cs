using Riok.Mapperly.Abstractions;
using ToDoApp.Data.Features.Auth.RefreshTokens;

namespace ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;

[Mapper]
public static partial class RefreshTokenEntityDtoMapper
{
    [MapperIgnoreSource(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.UserId))]
    public static partial RefreshTokenDto ToDto(this RefreshTokenEntity entity);
    public static partial IEnumerable<RefreshTokenDto> ToDtos(this IEnumerable<RefreshTokenEntity> entities);

    [MapperIgnoreSource(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreSource(nameof(RefreshTokenEntity.UserId))]
    public static partial void MapToDto(this RefreshTokenEntity source, RefreshTokenDto destination);
    public static partial void MapToDto(this RefreshTokenDto source, RefreshTokenDto destination);

    [MapperIgnoreTarget(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.UserId))]
    public static partial RefreshTokenEntity ToEntity(this RefreshTokenDto dto);
    public static partial IEnumerable<RefreshTokenDto> ToEntities(this IEnumerable<RefreshTokenEntity> dtos);

    [MapperIgnoreTarget(nameof(RefreshTokenEntity.Id))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.User))]
    [MapperIgnoreTarget(nameof(RefreshTokenEntity.UserId))]
    public static partial void MapToEntity(this RefreshTokenDto source, RefreshTokenEntity destination);
    public static partial void MapToEntity(this RefreshTokenEntity source, RefreshTokenEntity destination);
    public static partial IQueryable<RefreshTokenDto> ProjectToDto(this IQueryable<RefreshTokenEntity> query);
}