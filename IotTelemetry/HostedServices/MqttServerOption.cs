namespace IotTelemetry.HostedServices
{
    public class MqttServerOption : object
    {
        public string Hostname { get; set; } = default!;
        public int Port { get; set; } = default!;
        public string ClientId { get; set; } = default!;
    }
}
