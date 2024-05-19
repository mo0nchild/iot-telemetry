namespace IotTelemetry.HostedServices
{
    // Класс MqttServerOption, содержащий параметры конфигурации для подключения к MQTT-брокеру
    public class MqttServerOption : object
    {
        // Адрес хоста MQTT-брокера
        public string Hostname { get; set; } = default!;

        // Порт для подключения к MQTT-брокеру
        public int Port { get; set; } = default!;

        // Идентификатор клиента для подключения к MQTT-брокеру
        public string ClientId { get; set; } = default!;
    }
}
