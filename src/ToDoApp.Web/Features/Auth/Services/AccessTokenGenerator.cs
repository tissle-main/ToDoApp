using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Options;

namespace ToDoApp.Web.Features.Auth.Services;

public sealed class AccessTokenGenerator(
    UserManager<UserEntity> thisUserManager,
    IOptions<JwtOptions> thisJwtOptions
) : IAccessTokenGenerator
{
    #region Interfaces
    public async ValueTask<string> GenerateTokenAsync(UserEntity user, CancellationToken cancellationToken)
    {
        IList<string> roles = await thisUserManager.GetRolesAsync(user);
        List<Claim> claims = [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        ];
        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role))
        );
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(thisJwtOptions.Value.Key));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            issuer: thisJwtOptions.Value.Issuer,
            audience: thisJwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(thisJwtOptions.Value.ExpireMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    #endregion
}