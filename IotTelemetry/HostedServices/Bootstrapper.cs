namespace IotTelemetry.HostedServices
{
    public static class Bootstrapper : object
    {
        public static IServiceCollection AddMqttService(this IServiceCollection collection)
        {
            var config = collection.BuildServiceProvider().GetService<IConfiguration>();

            collection.AddHostedService<HiveMQService>();
            collection.Configure<MqttServerOption>(config!.GetSection("MqttBroker"));
            
            return collection;
        }
    }
}
