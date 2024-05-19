using IotTelemetry.Data.Entities;
using IotTelemetry.Infrastructure;
// ������������ ���� IotTelemetry.Data.Entities �������� ������ ���������, ������� ������������ ������, ������������ � ����������.
// ��� ������ ������������� �������� � ���� ������ � ������������ Entity Framework ��� �������� ������.
using IotTelemetry.Data.Entities;

// ������������ ���� IotTelemetry.Infrastructure �������� ���������� � ������ ���������������� �����������,
// ����� ��� ������� � �����������, ����������� ��� ������ ����������.
using IotTelemetry.Infrastructure;

// ������������ ���� IotTelemetry.Services �������� �������, ������� ��������� ������-������ ����������.
// � ������ ������, ��� ������ ��� ���������� ������� ������ � ��������.
using IotTelemetry.Services;

// ������������ ���� Microsoft.AspNetCore.Mvc �������� ������ � ���������� ��� �������� ������������ MVC � API,
// � ����� ��� ��������� HTTP-�������� � ������������ HTTP-�������.
using Microsoft.AspNetCore.Mvc;

// ������������ ���� Microsoft.EntityFrameworkCore �������� ������ � ���������� ��� ������ � Entity Framework Core,
// ORM (Object-Relational Mapping) ��� �������������� � ������ ������. ��� �������� ������ � �������,
// ������������ ��������������� ��������� ��� ���������� �������� � ������.
using Microsoft.EntityFrameworkCore;

// ������������ ���� Microsoft.Extensions.Caching.Memory �������� ������ � ���������� ��� ����������� ������ � ������,
// ��� ��������� �������� ��������� ������ � �������� ������������������ ����������. ��� ������ ��� �������� ������� � ����� ������������ ������.
using Microsoft.Extensions.Caching.Memory;

// ������������ ���� System.Linq �������� ������ � ���������� ��� ������ � ��������� LINQ (Language Integrated Query),
// ������� ��������� ������ ��������� �������� � C# ��� ������ � ����������� ������.
using System.Linq;

namespace IotTelemetry.Controllers
{
    // �������� ���������, ��� ���� ����� �������� ������������ API � ��� ������� ���������� � "telemetry"
    [ApiController]
    [Route("telemetry")]
    public class TelemetryController(ILogger<TelemetryController> logger, IMemoryCache cache, IAverageSensorService aveageSensorService) : ControllerBase
    {
        // ���� ��� �������� ������������, ���������� ����� ��������� ������������ (DI)
        private readonly ILogger<TelemetryController> _logger = logger;
        private readonly IMemoryCache _cache = cache;
        private readonly IAverageSensorService _averageSensorService = aveageSensorService;

        /// <summary>
        /// ���������� ������� ������ � ��������
        /// </summary>
        /// <returns>�����������, ���������, �����������</returns>
        [Route("current", Name = "GetCurrentSensors"), HttpGet]
        public IActionResult GetSensors()
        {
            // �������� ������� ������ � ����
            if (this._cache.TryGetValue("info", out var result))
            {
                // ���� ������ ����, ���������� �� � HTTP �������� 200 (OK)
                return this.Ok((Sensor)result!);
            }
            // ���� ������ ���, ���������� ������ � HTTP �������� 400 (BadRequest)
            return this.BadRequest("������ �� ���� ��������");
        }

        /// <summary>
        /// ���������� ������� ������ � �������� �� ����������� �������� �������
        /// </summary>
        /// <param name="fromDate">��������� ���� ���������</param>
        /// <param name="toDate">�������� ���� ���������</param>
        /// <returns>�����������, ���������, �����������</returns>
        [Route("average", Name = "GetAverageSensors"), HttpGet]
        public async Task<IActionResult> GetAverage([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            // ��������� ������� ������ � ������� �������
            var result = await _averageSensorService.GetAverageData(
                DateTime.Parse(fromDate).ToUniversalTime(),
                DateTime.Parse(toDate).ToUniversalTime());

            // ���� ������ �� �������� ���, �������� ��� � ���������� ������
            if (result is null)
            {
                this._logger.LogInformation($"\n\n\n\n{DateTime.Parse(fromDate).ToUniversalTime()}\n\n\n\n\n");
                return BadRequest("�� ������ �������� ��� ����������");
            }
            // ���� ������ ����, ���������� �� � HTTP �������� 200 (OK)
            return Ok(result);
        }
    }
}
