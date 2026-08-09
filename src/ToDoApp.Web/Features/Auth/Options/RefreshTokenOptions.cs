namespace ToDoApp.Web.Features.Auth.Options;

public sealed class RefreshTokenOptions
{
    #region Static
    public const string SectionName = "RefreshToken";
    #endregion

    #region Instance
    public required int RefreshTokenDurationInDays { get; set; }
    #endregion
}