using Riok.Mapperly.Abstractions;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Data.Features.Tasks_Categories;

namespace ToDoApp.Web.Features.Tasks.Dtos;

[Mapper]
public static partial class TaskEntityDtoMapper
{
    private static Guid GetCategoryId(Task_Category_JoinEntity entity)
    {
        return entity.RightId;
    }
    private static List<Task_Category_JoinEntity> GetJoinEntities(TaskDto dto)
    {
        return dto.Categories.Select(id => new Task_Category_JoinEntity()
        {
            LeftId = dto.Id,
            RightId = id
        }).ToList();
    }

    [MapperIgnoreSource(nameof(TaskEntity.User))]
    [MapperIgnoreSource(nameof(TaskEntity.UserId))]
    public static partial TaskDto ToDto(this TaskEntity entity);
    public static partial IEnumerable<TaskDto> ToDtos(this IEnumerable<TaskEntity> entities);

    [MapperIgnoreSource(nameof(TaskEntity.User))]
    [MapperIgnoreSource(nameof(TaskEntity.UserId))]
    public static partial void MapToDto(this TaskEntity source, TaskDto destination);
    public static partial void MapToDto(this TaskDto source, TaskDto destination);

    [MapperIgnoreTarget(nameof(TaskEntity.User))]
    [MapperIgnoreTarget(nameof(TaskEntity.UserId))]
    [MapPropertyFromSource(nameof(TaskEntity.Categories))]
    public static partial TaskEntity ToEntity(this TaskDto dto);
    public static partial IEnumerable<TaskDto> ToEntities(this IEnumerable<TaskEntity> dtos);

    [MapperIgnoreTarget(nameof(TaskEntity.User))]
    [MapperIgnoreTarget(nameof(TaskEntity.UserId))]
    [MapPropertyFromSource(nameof(TaskEntity.Categories))]
    public static partial void MapToEntity(this TaskDto source, TaskEntity destination);
    public static partial void MapToEntity(this TaskEntity source, TaskEntity destination);
    public static partial IQueryable<TaskDto> ProjectToDto(this IQueryable<TaskEntity> query);
}