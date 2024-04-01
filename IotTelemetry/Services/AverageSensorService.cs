using IotTelemetry.Data.Context;
using IotTelemetry.Data.Entities;
using IotTelemetry.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IotTelemetry.Services;

public class AverageSensorService(ILogger<AverageSensorService> logger, TelemetryDbContext context) : IAverageSensorService
{
    private readonly ILogger<AverageSensorService> _logger = logger;
    private readonly TelemetryDbContext _context = context;

    public async Task<Sensor?> GetAverageData(DateTime firstDate, DateTime secondDate)
    {
        if (!_context.SensorsData.Any(x => x.DateFetch == firstDate || x.DateFetch == secondDate))
            return null;

        var data = await _context.SensorsData.
            Where(x => x.DateFetch > firstDate && x.DateFetch < secondDate).
            ToListAsync();
        // TO DO: мб можно их как-нить в одну строку через LINQ, будто лучше циклом обычным, Average ток с одним может как я понял
        //var temperature = data.Average(x => x.Temperature);
        //var humidity = data.Average(x => x.Humidity);
        //var impurity = data.Average(x => x.Impurity);

        int count = data.Count;
        float temperature = default; float humidity = default; float impurity = default;

        foreach (var item in data)
        {
            temperature += item.Temperature;
            humidity += item.Humidity;
            impurity += item.Impurity;
        }
        temperature /= count; humidity /= count; impurity /= count;

        _logger.LogInformation($"Send data temperature: {temperature}| humidity {humidity}| impurity {impurity}");
        
        return new Sensor() { Humidity = humidity, Temperature = temperature, Impurity = impurity };
    } 
}
