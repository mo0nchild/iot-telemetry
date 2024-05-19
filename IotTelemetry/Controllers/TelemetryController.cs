using IotTelemetry.Data.Entities;
using IotTelemetry.Infrastructure;
// Пространство имен IotTelemetry.Data.Entities содержит классы сущностей, которые представляют данные, используемые в приложении.
// Эти классы соответствуют таблицам в базе данных и используются Entity Framework для маппинга данных.
using IotTelemetry.Data.Entities;

// Пространство имен IotTelemetry.Infrastructure содержит интерфейсы и классы инфраструктурных компонентов,
// таких как сервисы и репозитории, необходимые для работы приложения.
using IotTelemetry.Infrastructure;

// Пространство имен IotTelemetry.Services содержит сервисы, которые реализуют бизнес-логику приложения.
// В данном случае, это сервис для вычисления средних данных с датчиков.
using IotTelemetry.Services;

// Пространство имен Microsoft.AspNetCore.Mvc содержит классы и интерфейсы для создания контроллеров MVC и API,
// а также для обработки HTTP-запросов и формирования HTTP-ответов.
using Microsoft.AspNetCore.Mvc;

// Пространство имен Microsoft.EntityFrameworkCore содержит классы и интерфейсы для работы с Entity Framework Core,
// ORM (Object-Relational Mapping) для взаимодействия с базами данных. Это упрощает работу с данными,
// предоставляя высокоуровневый интерфейс для выполнения запросов и команд.
using Microsoft.EntityFrameworkCore;

// Пространство имен Microsoft.Extensions.Caching.Memory содержит классы и интерфейсы для кэширования данных в памяти,
// что позволяет временно сохранять данные и улучшить производительность приложений. Это удобно для быстрого доступа к часто используемым данным.
using Microsoft.Extensions.Caching.Memory;

// Пространство имен System.Linq содержит классы и интерфейсы для работы с запросами LINQ (Language Integrated Query),
// которые позволяют писать выражения запросов в C# для работы с коллекциями данных.
using System.Linq;

namespace IotTelemetry.Controllers
{
    // Атрибуты указывают, что этот класс является контроллером API и его маршрут начинается с "telemetry"
    [ApiController]
    [Route("telemetry")]
    public class TelemetryController(ILogger<TelemetryController> logger, IMemoryCache cache, IAverageSensorService aveageSensorService) : ControllerBase
    {
        // Поля для хранения зависимостей, полученных через внедрение зависимостей (DI)
        private readonly ILogger<TelemetryController> _logger = logger;
        private readonly IMemoryCache _cache = cache;
        private readonly IAverageSensorService _averageSensorService = aveageSensorService;

        /// <summary>
        /// Возвращает текущие данные с датчиков
        /// </summary>
        /// <returns>температура, влажность, загрязнение</returns>
        [Route("current", Name = "GetCurrentSensors"), HttpGet]
        public IActionResult GetSensors()
        {
            // Проверка наличия данных в кэше
            if (this._cache.TryGetValue("info", out var result))
            {
                // Если данные есть, возвращаем их с HTTP статусом 200 (OK)
                return this.Ok((Sensor)result!);
            }
            // Если данных нет, возвращаем ошибку с HTTP статусом 400 (BadRequest)
            return this.BadRequest("Данные не были получены");
        }

        /// <summary>
        /// Возвращает средние данные с датчиков за определённый интервал времени
        /// </summary>
        /// <param name="fromDate">Начальная дата интервала</param>
        /// <param name="toDate">Конечная дата интервала</param>
        /// <returns>температура, влажность, загрязнение</returns>
        [Route("average", Name = "GetAverageSensors"), HttpGet]
        public async Task<IActionResult> GetAverage([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            // Получение средних данных с помощью сервиса
            var result = await _averageSensorService.GetAverageData(
                DateTime.Parse(fromDate).ToUniversalTime(),
                DateTime.Parse(toDate).ToUniversalTime());

            // Если данных за интервал нет, логируем это и возвращаем ошибку
            if (result is null)
            {
                this._logger.LogInformation($"\n\n\n\n{DateTime.Parse(fromDate).ToUniversalTime()}\n\n\n\n\n");
                return BadRequest("За данный интервал нет информации");
            }
            // Если данные есть, возвращаем их с HTTP статусом 200 (OK)
            return Ok(result);
        }
    }
}
