using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ToDoApp.Data.Features.Auth.Users;

namespace ToDoApp.WebAPI.Services.Jwt;

public sealed class JwtService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions
) : IJwtService
{
    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);
        List<Claim> claims = [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        ];
        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role))
        );
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpireMinutes),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}