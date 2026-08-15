using Riok.Mapperly.Abstractions;
using ToDoApp.Data.Features.Categories;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Categories.Dtos;

[Mapper]
public static partial class CategoryEntityDtoMapper
{
    private static Guid GetTaskId(Task_Category_JoinEntity entity)
    {
        return entity.LeftId;
    }
    private static List<Task_Category_JoinEntity> GetJoinEntities(CategoryDto dto)
    {
        return dto.Tasks.Select(id => new Task_Category_JoinEntity()
        {
            LeftId = id,
            RightId = dto.Id
        }).ToList();
    }

    [MapperIgnoreSource(nameof(CategoryEntity.User))]
    [MapperIgnoreSource(nameof(CategoryEntity.UserId))]
    public static partial CategoryDto ToDto(this CategoryEntity entity);
    public static partial IEnumerable<CategoryDto> ToDtos(this IEnumerable<CategoryEntity> entities);

    [MapperIgnoreSource(nameof(CategoryEntity.User))]
    [MapperIgnoreSource(nameof(CategoryEntity.UserId))]
    public static partial void MapToDto(this CategoryEntity source, CategoryDto destination);
    public static partial void MapToDto(this CategoryDto source, CategoryDto destination);

    [MapperIgnoreTarget(nameof(CategoryEntity.User))]
    [MapperIgnoreTarget(nameof(CategoryEntity.UserId))]
    [MapPropertyFromSource(nameof(CategoryEntity.Tasks))]
    public static partial CategoryEntity ToEntity(this CategoryDto dto);
    public static partial IEnumerable<CategoryDto> ToEntities(this IEnumerable<CategoryEntity> dtos);

    [MapperIgnoreTarget(nameof(CategoryEntity.User))]
    [MapperIgnoreTarget(nameof(CategoryEntity.UserId))]
    [MapPropertyFromSource(nameof(CategoryEntity.Tasks))]
    public static partial void MapToEntity(this CategoryDto source, CategoryEntity destination);
    public static partial void MapToEntity(this CategoryEntity source, CategoryEntity destination);
    public static partial IQueryable<CategoryDto> ProjectToDto(this IQueryable<CategoryEntity> query);
}