using System.Text;
using System.IdentityModel.Tokens.Jwt;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Satlink.Api.Middleware;
using Satlink.Infrastructure.DI;
using Satlink.Logic.DI;
using Satlink.Logic;
using Satlink.Logic.Configuration;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "AngularDev";

// Add services to the container.
builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Return RFC 7807 response for validation failures.
        ValidationProblemDetails details = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7807",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(details);
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return false;
                }

                return Uri.TryCreate(origin, UriKind.Absolute, out Uri? uri)
                    && uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger/OpenAPI (Swashbuckle)
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Bind options.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
JwtOptions jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

const string IdentityServerScheme = "IdentityServer";
const string CombinedScheme = "CombinedJwtScheme";
string identityServerAuthority = builder.Configuration["IdentityServer:Authority"] ?? "https://localhost:5001";

// Configure authentication.
// AddPolicyScheme acts as a router: it inspects the token's 'iss' claim before
// any validation runs and forwards the request to the correct underlying scheme.
// This avoids the ambiguity of trying both schemes for every request and ensures
// that UseAuthentication() populates HttpContext.User with the right identity.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CombinedScheme;
    options.DefaultChallengeScheme = CombinedScheme;
})
.AddPolicyScheme(CombinedScheme, "Combined JWT Bearer", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string? authorization = context.Request.Headers.Authorization;

        if (!string.IsNullOrEmpty(authorization) &&
            authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            string token = authorization["Bearer ".Length..].Trim();
            try
            {
                JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                if (jwt.Issuer.StartsWith(identityServerAuthority, StringComparison.OrdinalIgnoreCase))
                    return IdentityServerScheme;
            }
            catch { }
        }

        return JwtBearerDefaults.AuthenticationScheme;
    };
})
.AddJwtBearer(options =>
{
    // Legacy symmetric-key tokens issued by AuthController.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
})
.AddJwtBearer(IdentityServerScheme, options =>
{
    // Tokens issued by Duende Identity Server.
    // Keys are downloaded automatically from the discovery document.
    options.Authority = identityServerAuthority;
    options.Audience = "satlink-api";
    options.RequireHttpsMetadata = false; // dev/mock — set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

// Register project dependencies.
builder.Services.RegisterInfrastructureDependencies(builder.Configuration);
builder.Services.RegisterLogicDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

// Global exception handler: must be registered early to catch all pipeline exceptions.
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();

// Request logger: registered after UseAuthentication so IUserContext has JWT claims.
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

// Ensure the SQL Server database and all EF Core tables (including MassTransit outbox
// tables: InboxState, OutboxMessage, OutboxState) are created before the app starts
// serving requests and before MassTransit's background services begin polling them.
await app.Services.InitializeSqlServerAsync();

app.MapControllers();

app.Run();
