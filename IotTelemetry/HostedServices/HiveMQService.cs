
using HiveMQtt.Client.Options;
using HiveMQtt.Client;
using Microsoft.Extensions.Caching.Memory;
using IotData.Entities;

namespace IotTelemetry.HostedServices;

public class HiveMQService : IHostedService
{
    protected readonly ILogger<HiveMQService> _logger = default!;
    protected readonly HiveMQClient _mqttClient = default!;
    protected readonly IMemoryCache _memoryCache = default!;
    public HiveMQService(ILogger<HiveMQService> logger, IMemoryCache memoryCache) : base() 
    {
        this._logger = logger;
        this._memoryCache = memoryCache;
        this._mqttClient = new HiveMQClient(new HiveMQClientOptions()
        {
            ClientId = "supertest",
            Host = "localhost",
            Port = 5000,
        });
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"Started");
        var item = new Indicator();
        while (!cancellationToken.IsCancellationRequested)
        {
            this._logger.LogInformation("Upload data tp broker");

            this._mqttClient.OnMessageReceived += (sender, args) =>
            {
                this._logger.LogInformation($"{args.PublishMessage.PayloadAsString}");
            };
            var response = await this._mqttClient.ConnectAsync();
            await this._mqttClient.SubscribeAsync("esp8266/data");

            await this._mqttClient.PublishAsync("esp8266/check", "alive");

            _memoryCache.Set<Indicator>("info", item,
                new MemoryCacheEntryOptions().
                    SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

            await Task.Delay(1000);
        }
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    this._logger.LogInformation($"Started at {DateTime.Now}");

    //    while (!stoppingToken.IsCancellationRequested)
    //    {
    //        this._logger.LogInformation("Fetch data");
    //        var item = new Indicator()
    //        {
    //            Humidity = new Random().Next(10, 30),
    //            Impurity = new Random().Next(10, 30),
    //            Temperature = new Random().Next(10, 30)
    //        };

    //        _memoryCache.Set<Indicator>("info", item,
    //            new MemoryCacheEntryOptions().
    //                SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

    //        await Task.Delay(1000);
    //    }
    //}
}
