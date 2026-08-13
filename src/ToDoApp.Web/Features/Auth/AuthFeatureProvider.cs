using ErrorOr;
using System.Text;
using ToDoApp.Data;
using ToDoApp.Data.Features.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using ToDoApp.Web.Features.Auth.Options;
using ToDoApp.Web.Features.Auth.Services;
using ToDoApp.Web.Features.Auth.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Auth;

public sealed class AuthFeatureProvider : FeatureProvider
{
    #region Interfaces
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddOptionsWithValidateOnStart<JwtOptions>().BindConfiguration(JwtOptions.SectionName);
        builder.Services.AddOptionsWithValidateOnStart<RefreshTokenOptions>().BindConfiguration(RefreshTokenOptions.SectionName);
        IConfigurationSection jwt = builder.Configuration.GetRequiredSection(JwtOptions.SectionName);
        builder.Services.AddIdentityCore<UserEntity>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = AuthConstants.PasswordMinLength;
            options.Password.RequireNonAlphanumeric = false;
        }).AddRoles<RoleEntity>().AddSignInManager().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            string jwtKey = jwt[nameof(JwtOptions.Key)]!;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt[nameof(JwtOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwt[nameof(JwtOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.AddRegisterUserEndpoint();
        app.AddLoginUserEndpoint();
        app.AddRefreshAccessTokenEndpoint();
        app.AddDeleteUserEndpoint();
    }
    #endregion
}
public static class AuthConstants
{
    public const int PasswordMinLength = 8;
    [StringSyntax(StringSyntaxAttribute.Regex)] public const string PasswordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{8,}$";
    public const string PasswordValidationMessage = "Password must contain at least 8 characters, including uppercase, lowercase, and a number.";
}
public static class AuthErrors
{
    public static Error UserExists()
    {
        return Error.Conflict("Auth.UserExists", "User already exists.");
    }
    public static Error UserNotFound()
    {
        return Error.NotFound("Auth.UserNotFound", "User not found.");
    }
    public static Error UserLockedOut()
    {
        return Error.Failure("Auth.UserLockedOut", "User locked out.");
    }
    public static Error UserNotAllowed()
    {
        return Error.Failure("Auth.UserNotAllowed", "User not allowed to sign in.");
    }
    public static Error Requires2FA()
    {
        return Error.Failure("Auth.Requires2FA", "Sign in requires 2FA.");
    }
    public static Error InvalidPassword()
    {
        return Error.Failure("Auth.InvalidPassword", "Entered invalid password.");
    }
    public static Error RefreshTokenNotFound()
    {
        return Error.NotFound("Auth.RefreshTokenNotFound", "Refresh token not found.");
    }
    public static Error RefreshTokenExpired()
    {
        return Error.Failure("Auth.RefreshTokenExpired", "Refresh token expired.");
    }

    public static IEnumerable<Error> ToErrors(this IdentityResult result)
    {
        if(result.Succeeded)
        {
            return [];
        }
        return result.Errors.Select(error => Error.Failure(error.Code, error.Description));
    }
    public static Error ToError(this SignInResult result)
    {
        if(result.IsLockedOut)
        {
            return UserLockedOut();
        }
        if(result.IsNotAllowed)
        {
            return UserNotAllowed();
        }
        if(result.RequiresTwoFactor)
        {
            return Requires2FA();
        }
        return InvalidPassword();
    }
}