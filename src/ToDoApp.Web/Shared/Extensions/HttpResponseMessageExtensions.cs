using Microsoft.Net.Http.Headers;

namespace ToDoApp.Web.Shared.Extensions;

public static class HttpResponseMessageExtensions
{
    public const string CookieHeader = "Set-Cookie";

    extension(HttpResponseMessage thisHttpResponse)
    {
        public string? GetCookie(string key)
        {
            if(!thisHttpResponse.Headers.TryGetValues(CookieHeader, out IEnumerable<string>? cookies))
            {
                return null;
            }
            foreach(string cookieHeader in cookies)
            {
                SetCookieHeaderValue cookie = SetCookieHeaderValue.Parse(cookieHeader);
                if(cookie.Name == key)
                {
                    return cookie.Value.ToString();
                }
            }
            return null;
        }
        public string? GetRefreshToken()
        {
            string? refreshToken = thisHttpResponse.GetCookie(HttpContextExtensions.RefreshTokenKey);
            if(refreshToken is not null)
            {
                return Uri.UnescapeDataString(refreshToken);
            }
            return refreshToken;
        }
    }
}