
using HiveMQtt.Client.Options;
using HiveMQtt.Client;

namespace IotTelemetry.HostedServices
{
    public class HiveMQService : IHostedService
    {
        protected readonly ILogger<HiveMQService> _logger = default!;
        protected readonly HiveMQClient mqttClient = default!;
        public HiveMQService(ILogger<HiveMQService> logger) : base() 
        {
            this._logger = logger;
            this.mqttClient = new HiveMQClient(new HiveMQClientOptions()
            {
                ClientId = "supertest",
                Host = "localhost",
                Port = 5000,
            });
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"Started");
            this.mqttClient.OnMessageReceived += (sender, args) =>
            {
                this._logger.LogInformation($"{args.PublishMessage.PayloadAsString}");
            };
            var response = await this.mqttClient.ConnectAsync();
            await this.mqttClient.SubscribeAsync("esp8266/data");

            await this.mqttClient.PublishAsync("esp8266/check", "alive");
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
