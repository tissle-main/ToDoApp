using Riok.Mapperly.Abstractions;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.Web.Features.Auth.Dtos.Users;

[Mapper]
public static partial class UserEntityDtoMapper
{
    public static UserDto ToDto(this UserEntity entity)
    {
        return new UserDto()
        {
            Email = entity.Email!
        };
    }
    public static partial IEnumerable<UserDto> ToDtos(this IEnumerable<UserEntity> entities);
    public static void MapToDto(this UserEntity source, UserDto destination)
    {
        destination.Email = source.Email!;
    }
    public static partial void MapToDto(this UserDto source, UserDto destination);
    public static partial IQueryable<UserDto> ProjectToDto(this IQueryable<UserEntity> query);
}