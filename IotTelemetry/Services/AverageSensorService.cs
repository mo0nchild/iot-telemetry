using IotTelemetry.Data.Context;
using IotTelemetry.Data.Entities;
using IotTelemetry.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IotTelemetry.Services;

// Определение класса AverageSensorService, реализующего интерфейс IAverageSensorService
public class AverageSensorService(ILogger<AverageSensorService> logger,
    IDbContextFactory<TelemetryDbContext> factory) : IAverageSensorService
{
    // Поля для хранения зависимостей
    private readonly ILogger<AverageSensorService> _logger = logger;
    private readonly IDbContextFactory<TelemetryDbContext> _factory = factory;

    // Метод для получения средних данных датчиков за заданный период
    public async Task<Sensor?> GetAverageData(DateTime firstDate, DateTime secondDate)
    {
        // Создание контекста базы данных
        using var _context = await this._factory.CreateDbContextAsync();

        // Получение данных датчиков за заданный период
        var data = await _context.SensorsData
            .Where(x => x.DateFetch > firstDate && x.DateFetch < secondDate)
            .ToListAsync();

        // Инициализация переменных для подсчета средних значений
        int count = data.Count;
        float temperature = default;
        float humidity = default;
        float impurity = default;

        // Подсчет суммы всех значений
        foreach (var item in data)
        {
            temperature += item.Temperature;
            humidity += item.Humidity;
            impurity += item.Impurity;
        }

        // Вычисление средних значений
        temperature /= count;
        humidity /= count;
        impurity /= count;

        // Логирование полученных средних значений
        _logger.LogInformation($"Send data temperature: {temperature}| humidity {humidity}| impurity {impurity}");

        // Возвращение нового объекта Sensor с рассчитанными средними значениями
        return new Sensor() { Humidity = humidity, Temperature = temperature, Impurity = impurity };
    }
}
