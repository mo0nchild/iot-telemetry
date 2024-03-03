using IotData.Context;
using IotData.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IotTelemetry.Controllers;

[ApiController]
[Route("[controller]")]
public class TelemetryController(ILogger<TelemetryController> logger, IMemoryCache cache) : ControllerBase
{
    private readonly ILogger<TelemetryController> _logger = logger;
    private readonly IMemoryCache _cache = cache;

    /// <summary>
    /// Возвращает данные с датчиков
    /// </summary>
    /// <returns>температура, влажность, загрязнение</returns>
    [HttpGet(Name = "get_indicators")]
    public IActionResult TestDb()
    {
        _logger.LogInformation($"Fetch data at {DateTime.UtcNow}");
        return Ok((Indicator)_cache.Get("info")!);
    }
}
