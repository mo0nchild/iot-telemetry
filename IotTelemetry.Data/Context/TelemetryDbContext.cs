using IotTelemetry.Data.Configuration;
using IotTelemetry.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IotTelemetry.Data.Context;

public partial class TelemetryDbContext : DbContext
{
    public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options)
        : base(options) => Database.EnsureCreated();

    public virtual DbSet<Sensor> SensorsData { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureSensor();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
