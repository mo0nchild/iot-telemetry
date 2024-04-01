using IotTelemetry.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotTelemetry.Data.Configuration;
public static class SensorsConfiguration
{
    public static void ConfigureSensor(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sensors_data_pkey");

            entity.ToTable("sensors_data");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateFetch)
                .HasDefaultValueSql("now()")
                .HasColumnName("date_fetch");
            entity.Property(e => e.Humidity).HasColumnName("humidity");
            entity.Property(e => e.Impurity).HasColumnName("impurity");
            entity.Property(e => e.Temperature).HasColumnName("temperature");
        });
    }
}
