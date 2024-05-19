using IotTelemetry.Data.Configuration;
using IotTelemetry.Data.Entities;
// Это основная библиотека Entity Framework Core, которая предоставляет функциональность для работы с базами данных в .NET Core приложениях.
using Microsoft.EntityFrameworkCore;

namespace IotTelemetry.Data.Context
{
    // Класс TelemetryDbContext представляет контекст базы данных для взаимодействия с таблицами данных.
    public partial class TelemetryDbContext : DbContext
    {
        // Конструктор класса, принимающий параметры конфигурации базы данных и вызывающий метод EnsureCreated() для создания базы данных (если она не существует).
        public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : base(options) => Database.EnsureCreated();

        // Свойство DbSet<Sensor> представляет набор сущностей Sensor, которые могут быть использованы для выполнения запросов к соответствующей таблице в базе данных.
        public virtual DbSet<Sensor> SensorsData { get; set; }

        // Метод OnModelCreating переопределяет базовую реализацию для настройки моделей данных с помощью Fluent API.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Вызов метода ConfigureSensor(), определенного в классе SensorConfiguration, для настройки сущности Sensor.
            modelBuilder.ConfigureSensor();

            // Частичный метод для дополнительной настройки моделей данных (может быть определен в других частичных классах).
            OnModelCreatingPartial(modelBuilder);
        }

        // Частичный метод, который может быть реализован в других частичных классах для дополнительной настройки моделей данных.
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
