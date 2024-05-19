using IotTelemetry.Infrastructure;
using IotTelemetry.Services;

namespace IotTelemetry.HostedServices
{
    // Статический класс Bootstrapper для настройки служб
    public static class Bootstrapper : object
    {
        // Метод расширения для добавления службы MQTT в коллекцию служб
        public static IServiceCollection AddMqttService(this IServiceCollection collection)
        {
            // Получение конфигурации из коллекции служб
            var config = collection.BuildServiceProvider().GetService<IConfiguration>();

            // Добавление хостируемой службы HiveMQService
            collection.AddHostedService<HiveMQService>();

            // Конфигурация параметров сервера MQTT из раздела "MqttBroker" конфигурации
            collection.Configure<MqttServerOption>(config!.GetSection("MqttBroker"));

            // Возвращение обновленной коллекции служб
            return collection;
        }

        // Метод расширения для добавления службы AverageSensorService в коллекцию служб
        public static IServiceCollection AddAvgService(this IServiceCollection collection)
            // Добавление службы AverageSensorService с временным временем жизни (Transient)
            => collection.AddTransient<IAverageSensorService, AverageSensorService>();
    }
}
