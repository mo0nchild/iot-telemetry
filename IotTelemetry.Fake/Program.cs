using IotTelemetry.Fake.HostedServices;
using IotTelemetry.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IotTelemetry.Fake
{
    internal class Program : object
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.AddJsonFile("appsettings.json");
            });
            builder.ConfigureServices(services =>
            {
                var config = services.BuildServiceProvider().GetService<IConfiguration>();
                services.Configure<MqttServerOption>(config!.GetSection("MqttBroker"));
                services.AddHostedService<PublishDataService>();
            });
            var application = builder.Build();
            application.Run();
        }
    }
}
