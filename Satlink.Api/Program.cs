using System.Text;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Satlink.Api.Middleware;
using Satlink.Api.Services;
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

// Configure authentication.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
    });

builder.Services.AddAuthorization();

// IHttpContextAccessor is required by UserContext to read JWT claims.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

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
