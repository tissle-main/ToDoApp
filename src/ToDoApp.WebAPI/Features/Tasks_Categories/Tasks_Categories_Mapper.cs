using ToDoApp.WebAPI.Features.Tasks.Dtos;
using ToDoApp.Data.Features.Tasks_Categories;
using ToDoApp.WebAPI.Features.Categories.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks_Categories;

public static class Tasks_Categories_Mapper
{
    public static TaskDto ToTaskDto(this Task_Category_JoinEntity entity)
    {
        return entity.Task!.ToDto();
    }
    public static List<Task_Category_JoinEntity> ToJoinEntities(this TaskDto dto)
    {
        return dto.Categories.Select(c => new Task_Category_JoinEntity()
        {
            TaskId = dto.Id,
            CategoryId = c.Id,
        }).ToList();
    }
    public static CategoryDto ToCategoryDto(this Task_Category_JoinEntity entity)
    {
        return entity.Category!.ToDto();
    }
    public static List<Task_Category_JoinEntity> ToJoinEntities(this CategoryDto dto)
    {
        return dto.Tasks.Select(t => new Task_Category_JoinEntity()
        {
            TaskId = t.Id,
            CategoryId = dto.Id
        }).ToList();
    }
}