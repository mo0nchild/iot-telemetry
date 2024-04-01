using IotTelemetry.Data.Entities;

namespace IotTelemetry.Infrastructure;

public interface IAverageSensorService
{
    public Task<Sensor?> GetAverageData(DateTime firstDate, DateTime secondDate);
}
