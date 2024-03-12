
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using IotTelemetry.HostedServices;

namespace IotTelemetry
{
    internal class Program : object
    {
        public Program() : base() { }
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}
