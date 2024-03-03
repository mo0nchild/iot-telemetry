
using HiveMQtt.Client.Options;
using HiveMQtt.Client;
using Microsoft.Extensions.Caching.Memory;
using IotData.Entities;

namespace IotTelemetry.HostedServices;

public class HiveMQService : BackgroundService
{
    protected readonly ILogger<HiveMQService> _logger = default!;
    protected readonly HiveMQClient _mqttClient = default!;
    protected readonly IMemoryCache _memoryCache = default!;
    private Indicator Item = new();

    public HiveMQService(ILogger<HiveMQService> logger, IMemoryCache memoryCache) : base()
    {
        this._logger = logger;
        this._memoryCache = memoryCache;
        this._mqttClient = new HiveMQClient(new HiveMQClientOptions()
        {
            ClientId = "supertest",
            Host = "10.241.0.1",
            Port = 5000,
        });
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            this._mqttClient.OnMessageReceived += (sender, args) =>
            {
                this._logger.LogInformation($"{args.PublishMessage.PayloadAsString}");
                var str = args.PublishMessage.PayloadAsString.Split(" ");

                Item.Temperature = float.Parse(str[1].Trim(';'));
                Item.Humidity = float.Parse(str[3].Trim(';'));
                Item.Impurity = float.Parse(str[5]);

                _memoryCache.Set<Indicator>("info", Item,
                    new MemoryCacheEntryOptions().
                        SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            };
            await this._mqttClient.ConnectAsync();

            _logger.LogInformation("Подключение установлено");

            await this._mqttClient.SubscribeAsync("esp8266/data");
        }
        catch { _logger.LogError("Подключение не было установлено"); }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(200, stoppingToken);
        }
        await Task.CompletedTask;
    }

}
