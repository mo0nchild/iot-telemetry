using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using IotTelemetry.HostedServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotTelemetry.Fake.HostedServices
{
    internal class PublishDataService : BackgroundService
    {
        protected readonly ILogger<PublishDataService> _logger = default!;
        private readonly IHiveMQClient _mqttClient = default!;
        public PublishDataService(IOptions<MqttServerOption> options,
            ILogger<PublishDataService> logger) : base() 
        {
            this._mqttClient = new HiveMQClient(new HiveMQClientOptions()
            {
                Host = options.Value.Hostname,
                Port = options.Value.Port,
                ClientId = options.Value.ClientId,
            });
            this._logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var response = await this._mqttClient.ConnectAsync();
            while (!stoppingToken.IsCancellationRequested)
            {
                await this._mqttClient.PublishAsync("esp8266/data", $"temp: {36}; hum: {75}; ppm: {3000}");
                await Task.Delay(2000);
            }
        }
    }
}
