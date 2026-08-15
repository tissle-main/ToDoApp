namespace ToDoApp.Data.Features.Auth.Users.ForeignKey;

public interface IUserEntityForeignKey
{
    //Value properties
    public abstract Guid UserId { get; set; }

    //Navigation properties
    public abstract UserEntity? User { get; set; }
}