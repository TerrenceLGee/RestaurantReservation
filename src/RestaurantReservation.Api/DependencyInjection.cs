using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

using RestaurantReservation.Api.Handlers;
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

        throw new NotImplementedException();
    }
}