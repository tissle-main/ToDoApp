namespace ToDoApp.Web.Features.Auth.Options;

public sealed class JwtOptions
{
    #region Static
    public const string SectionName = "Jwt";
    #endregion

    #region Instance
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Key { get; init; }
    public required int ExpireMinutes { get; init; }
    #endregion
}