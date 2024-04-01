using IotTelemetry.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IotTelemetry.Data;

public static class Bootstrapper
{
    public static IServiceCollection AddDataBase(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetService<IConfiguration>();
        services.AddDbContextFactory<TelemetryDbContext>(options =>
        {
            options.UseNpgsql(config!.GetConnectionString("telemetrydb"));
        });
        return services;
    }
}
