// Пространство имен IotTelemetry.Data.Context содержит классы, представляющие контекст базы данных,
// который управляет доступом к базе данных и операциями с данными с использованием Entity Framework Core.
using IotTelemetry.Data.Context;

// Пространство имен Microsoft.EntityFrameworkCore содержит классы и интерфейсы для работы с Entity Framework Core,
// ORM (Object-Relational Mapping) для взаимодействия с базами данных. Это упрощает работу с данными,
// предоставляя высокоуровневый интерфейс для выполнения запросов и команд.
using Microsoft.EntityFrameworkCore;

// Пространство имен Microsoft.Extensions.Configuration содержит интерфейсы и классы для работы с конфигурацией приложения,
// включая получение значений конфигурации из различных источников, таких как файлы конфигурации или переменные среды.
using Microsoft.Extensions.Configuration;

// Пространство имен Microsoft.Extensions.DependencyInjection содержит классы и интерфейсы для внедрения зависимостей в ASP.NET Core.
// Это позволяет легко управлять зависимостями и создавать объекты служб для использования в приложении.
using Microsoft.Extensions.DependencyInjection;

namespace IotTelemetry.Data
{
    // Статический класс Bootstrapper, содержащий методы для настройки сервисов базы данных
    public static class Bootstrapper
    {
        // Метод расширения для добавления сервисов базы данных в контейнер зависимостей
        public static IServiceCollection AddDataBase(this IServiceCollection services)
        {
            // Получение конфигурации из контейнера зависимостей
            var config = services.BuildServiceProvider().GetService<IConfiguration>();

            // Добавление фабрики контекста базы данных TelemetryDbContext в контейнер зависимостей
            services.AddDbContextFactory<TelemetryDbContext>(options =>
            {
                // Настройка используемой базы данных и строки подключения через конфигурацию
                options.UseNpgsql(config!.GetConnectionString("telemetrydb"));
            });

            // Получение фабрики контекста из контейнера зависимостей
            var factory = services.BuildServiceProvider().GetService<IDbContextFactory<TelemetryDbContext>>();

            // Применение миграций к базе данных для обновления структуры, если это необходимо
            // (например, при первом запуске приложения)
            using (var dbcontext = factory!.CreateDbContext())
            {
                dbcontext.Database.Migrate();
            }

            // Возврат обновленной коллекции сервисов
            return services;
        }
    }
}
