using IotTelemetry.Data.Entities;
using IotTelemetry.Infrastructure;
using IotTelemetry.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace IotTelemetry.Controllers;

[ApiController]
[Route("telemetry")]
public class TelemetryController(ILogger<TelemetryController> logger, IMemoryCache cache, IAverageSensorService aveageSensorService) : ControllerBase
{
    private readonly ILogger<TelemetryController> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly IAverageSensorService _averageSensorService = aveageSensorService;
    /// <summary>
    /// ���������� ������ � ��������
    /// </summary>
    /// <returns>�����������, ���������, �����������</returns>
    [Route("current", Name = "GetCurrentSensors"), HttpGet]
    public IActionResult GetSensors()
    {
        if (this._cache.TryGetValue("info", out var result))
        {
            return this.Ok((Sensor)result!);
        }
        return this.BadRequest("������ �� ���� ��������");
    }
    /// <summary>
    /// ���������� ������ � �������� �� ����������� ��������
    /// </summary>
    /// <param name="firstDate">������ ����</param>
    /// <param name="secondDate">������ ����</param>
    /// <returns>�����������, ���������, �����������</returns>
    [Route("average", Name ="GetAverageSensors"),HttpGet]
    public IActionResult GetAverage(DateTime firstDate, DateTime secondDate)
    {
        var result = _averageSensorService.GetAverageData(firstDate, secondDate);

        if (result is null)
        {
            return BadRequest("�� ������ �������� ��� ����������");
        }
        return Ok(result);
    }
}
