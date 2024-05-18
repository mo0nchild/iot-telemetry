using IotTelemetry.Data.Entities;

namespace IotTelemetry.Infrastructure;
/// <summary>
/// Интерфейса для сервиса обработки данных с датчиков
/// </summary>
public interface IAverageSensorService
{
    /// <summary>
    /// Метод для получения средних данных датчиков за заданный период
    /// </summary>
    /// <param name="firstDate">Начальная дата периода</param>
    /// <param name="secondDate">Конечная дата периода</param>
    /// <returns>Задача, возвращающая объект Sensor с рассчитанными средними значениями или null</returns>
    public Task<Sensor?> GetAverageData(DateTime firstDate, DateTime secondDate);
}