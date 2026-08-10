using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.OpenApi;

using RestaurantReservation.Api.Handlers;
using RestaurantReservation.Application.Abstractions;
using RestaurantReservation.Infrastructure.Persistence;

using Serilog;

namespace RestaurantReservation.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSerilog((srvs, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(srvs));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                var info = document.Info;
                info.Title = "Restaurant Reservation Api";
                info.Description = "An API for Creating, Cancelling, or Updating Restaurant Reservations";
                info.Contact = new OpenApiContact
                {
                    Name = "Terrence L. Gee",
                    Email = "mrgee1978@proton.me",
                    Url = new Uri("https://github.com/TerrenceLGee?tab=repositories")
                };
                document.Info = info;

                var components = document.Components ?? new OpenApiComponents();
                components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token"
                };

                document.Components = components;

                var schemeReference = new OpenApiSecuritySchemeReference("Bearer");
                var securityRequirement = new OpenApiSecurityRequirement { [schemeReference] = [] };

                document.Security ??= [];
                document.Security.Add(securityRequirement);
                return Task.CompletedTask;
            });
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "restaurantreservationapi:";
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5), Expiration = TimeSpan.FromMinutes(30)
            };
            options.MaximumPayloadBytes = 1024 * 1024;
        });

        services.Configure<SmtpOptions>(
            configuration.GetSection("Smtp"));
        return services;
    }
}