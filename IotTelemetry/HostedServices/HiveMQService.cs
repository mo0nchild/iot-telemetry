
using HiveMQtt.Client.Options;
using HiveMQtt.Client;
using Microsoft.Extensions.Caching.Memory;
using IotData.Entities;
using System.Threading;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace IotTelemetry.HostedServices;

public class HiveMQService : BackgroundService
{
    protected readonly ILogger<HiveMQService> _logger = default!;
    protected readonly HiveMQClient _mqttClient = default!;
    protected readonly IMemoryCache _memoryCache = default!;
    public HiveMQService(ILogger<HiveMQService> logger, 
        IMemoryCache memoryCache,
        IOptions<MqttServerOption> options) : base() 
    {
        this._logger = logger;
        this._memoryCache = memoryCache;
        this._mqttClient = new HiveMQClient(new HiveMQClientOptions()
        {
            Host = options.Value.Hostname,
            Port = options.Value.Port,
            ClientId = options.Value.ClientId,
        });
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation($"Connection to MQTT broker: {this._mqttClient.Options.Host}");
        this._mqttClient.OnMessageReceived += (sender, args) =>
        {
            //this._logger.LogInformation($"{args.PublishMessage.PayloadAsString}");
            var str = args.PublishMessage.PayloadAsString.Split("; ");
            try {
                var item = new Indicator()
                {
                    Temperature = float.Parse(str[0].Replace("temp: ", "").Replace(".", ",")),
                    Humidity = float.Parse(str[1].Replace("hum: ", "").Replace(".", ",")),
                    Impurity = float.Parse(str[2].Replace("ppm: ", "").Replace(".", ",")),
                };
                var dataTimer = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                this._memoryCache.Set<Indicator>("info", item, dataTimer);
            }
            catch(Exception error) { this._logger.LogWarning(error.Message); }
        };
        var response = await this._mqttClient.ConnectAsync();
        await this._mqttClient.SubscribeAsync("esp8266/data");
        while (!stoppingToken.IsCancellationRequested)
        {
            //this._logger.LogInformation("Upload data to broker");
            await this._mqttClient.PublishAsync("esp8266/check", "alive");

            await Task.Delay(1000);
        }
    }
}
