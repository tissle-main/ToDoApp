using ToDoApp.Web.Features.Auth.Dtos.RefreshTokens;
using ToDoApp.Web.Features.Auth.Handlers.RefreshAccessToken;

namespace ToDoApp.Web.Shared.Extensions;

public static class HttpContextExtensions
{
    public const string RefreshTokenKey = "refresh-token";

    extension(HttpContext thisHttpContext)
    {
        public void AddRefreshToken(RefreshTokenDto refreshToken)
        {
            thisHttpContext.Response.Cookies.Append(RefreshTokenKey, refreshToken.Value, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.ExpiresAt,
                Path = RefreshAccessTokenEndpoint.Url
            });
        }
        public string? GetRefreshToken()
        {
            return thisHttpContext.Request.Cookies[RefreshTokenKey];
        }
    }
}