using ErrorOr;
using Mediator;
using System.Text;
using ToDoApp.Data;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Web.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Data.Features.Auth.Roles;
using ToDoApp.Data.Features.Auth.Users;
using ToDoApp.Web.Features.Auth.Options;
using ToDoApp.Web.Features.Auth.Services;
using ToDoApp.Web.Features.Auth.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ToDoApp.Web.Features.Auth;

public sealed class AuthFeatureProvider : IFeatureProvider
{
    #region Static
    public static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static async Task<IResult> LoginUser(
        [FromBody] LoginUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<LoginUserResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static async Task<IResult> RefreshAccessToken(
        [FromBody] RefreshAccessTokenCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<RefreshAccessTokenResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static async Task<IResult> DeleteUser(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> result = await mediator.Send(new DeleteUserCommand(), cancellationToken);
        return result.ToHttpResult();
    }
    #endregion

    #region Interfaces
    public void AddFeature(IHostApplicationBuilder builder)
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
    public void UseFeature(IApplicationBuilder builder)
    {
        builder.UseAuthentication();
        builder.UseAuthorization();
    }
    public void MapEndpoints(IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/auth");
        group.MapPost("/register", RegisterUser)
            .WithName(nameof(RegisterUser))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        group.MapPost("/login", LoginUser)
            .WithName(nameof(LoginUser))
            .Produces<LoginUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
        group.MapPut("/refresh", RefreshAccessToken)
            .WithName(nameof(RefreshAccessToken))
            .Produces<RefreshAccessTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        group.MapDelete("/delete", DeleteUser).RequireAuthorization()
            .WithName(nameof(DeleteUser))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
    #endregion
}