using IotTelemetry.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace IotTelemetry.Controllers;

[ApiController]
[Route("telemetry")]
public class TelemetryController(ILogger<TelemetryController> logger, IMemoryCache cache) : ControllerBase
{
    private readonly ILogger<TelemetryController> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    /// <summary>
    /// Возвращает данные с датчиков
    /// </summary>
    /// <returns>температура, влажность, загрязнение</returns>
    [Route("sensors", Name = "GetSensors"), HttpGet]
    public IActionResult GetSensors()
    {
        if (this._cache.TryGetValue("info", out var result))
        {
            return this.Ok((Indicator)result!);
        }
        return this.BadRequest("Данные не были получены");
        
    }
}
