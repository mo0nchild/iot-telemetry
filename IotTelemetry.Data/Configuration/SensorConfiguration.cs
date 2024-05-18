using IotTelemetry.Data.Entities; //Это библиотека с сущностью бд
using Microsoft.EntityFrameworkCore; //Это основная библиотека Entity Framework Core, которая предоставляет функциональность для работы с базами данных в .NET Core приложениях. 

namespace IotTelemetry.Data.Configuration
{
    // Статический класс SensorsConfiguration, содержащий метод расширения для настройки сущности Sensor
    public static class SensorsConfiguration
    {
        // Метод ConfigureSensor предоставляет настройки для сущности Sensor с использованием Fluent API
        public static void ConfigureSensor(this ModelBuilder modelBuilder)
        {
            // Настраиваем сущность Sensor
            modelBuilder.Entity<Sensor>(entity =>
            {
                // Устанавливаем первичный ключ
                entity.HasKey(e => e.Id).HasName("sensors_data_pkey");

                // Задаем имя таблицы базы данных, с которой связана сущность
                entity.ToTable("sensors_data");

                // Настраиваем свойства сущности, определяя их названия и типы
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DateFetch).HasColumnName("date_fetch");
                entity.Property(e => e.Humidity).HasColumnName("humidity");
                entity.Property(e => e.Impurity).HasColumnName("impurity");
                entity.Property(e => e.Temperature).HasColumnName("temperature");
            });
        }
    }
}
