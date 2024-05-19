// Пространство имен HiveMQtt.Client.Options содержит классы и интерфейсы для конфигурации опций клиента HiveMQTT.
using HiveMQtt.Client.Options;

// Пространство имен HiveMQtt.Client содержит классы и интерфейсы для работы с клиентом HiveMQTT, 
// включая подключение, подписку и публикацию сообщений.
using HiveMQtt.Client;

// Пространство имен Microsoft.Extensions.Caching.Memory содержит классы и интерфейсы для кэширования данных в памяти, 
// что позволяет временно сохранять данные и улучшить производительность приложений.
using Microsoft.Extensions.Caching.Memory;

// Пространство имен Microsoft.Extensions.Options содержит классы и интерфейсы для настройки параметров конфигурации, 
// включая работу с параметрами конфигурации через объект IOptions<T>.
using Microsoft.Extensions.Options;

// Пространство имен IotTelemetry.Data.Entities содержит классы сущностей, которые представляют данные, 
// используемые в приложении, такие как объекты датчиков.
using IotTelemetry.Data.Entities;

// Пространство имен IotTelemetry.Data.Context содержит классы контекста базы данных, 
// которые управляют доступом к базе данных и операциями с данными.
using IotTelemetry.Data.Context;

// Пространство имен Microsoft.EntityFrameworkCore содержит классы и интерфейсы для работы с Entity Framework Core, 
// ORM (Object-Relational Mapping) для взаимодействия с базами данных.
using Microsoft.EntityFrameworkCore;

namespace IotTelemetry.HostedServices;

// Класс HiveMQService, наследующийся от BackgroundService для выполнения фоновых задач
public class HiveMQService : BackgroundService
{
    // Поля для хранения зависимостей
    protected readonly ILogger<HiveMQService> _logger = default!;
    protected readonly HiveMQClient _mqttClient = default!;
    protected readonly IMemoryCache _memoryCache = default!;
    protected readonly IDbContextFactory<TelemetryDbContext> _factory = default!;

    // Конструктор класса HiveMQService, принимающий зависимости через внедрение зависимостей (DI)
    public HiveMQService(ILogger<HiveMQService> logger,
        IMemoryCache memoryCache,
        IOptions<MqttServerOption> options,
        IDbContextFactory<TelemetryDbContext> factory
        ) : base()
    {
        // Инициализация полей зависимостями
        this._logger = logger;
        this._memoryCache = memoryCache;
        this._factory = factory;
        // Создание клиента HiveMQClient с настройками из конфигурации
        this._mqttClient = new HiveMQClient(new HiveMQClientOptions()
        {
            Host = options.Value.Hostname,
            Port = options.Value.Port,
            ClientId = options.Value.ClientId,
        });
    }

    // Метод ExecuteAsync, выполняемый при запуске службы
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Логирование информации о подключении к MQTT-брокеру
        this._logger.LogInformation($"Connection to MQTT broker: {this._mqttClient.Options.Host}");
        this._logger.LogInformation($"MQTT Broker port: {this._mqttClient.Options.Port}");

        // Обработчик события получения сообщения от MQTT-брокера
        this._mqttClient.OnMessageReceived += async (sender, args) =>
        {
            // Разделение полученного сообщения на части
            var str = args.PublishMessage.PayloadAsString.Split("; ");
            try
            {
                // Создание нового объекта Sensor с данными из сообщения
                var item = new Sensor()
                {
                    Temperature = float.Parse(str[0].Replace("temp: ", "").Replace(".", ".")),
                    Humidity = float.Parse(str[1].Replace("hum: ", "").Replace(".", ".")),
                    Impurity = float.Parse(str[2].Replace("ppm: ", "").Replace(".", ".")),
                    DateFetch = DateTime.Now
                };

                // Настройка кэширования данных
                var dataTimer = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                this._memoryCache.Set<Sensor>("info", item, dataTimer);

                // Использование фабрики контекстов для сохранения данных в базу данных
                using (var _context = await this._factory.CreateDbContextAsync())
                {
                    item.DateFetch = DateTime.UtcNow;
                    await _context.SensorsData.AddAsync(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception error)
            {
                // Логирование ошибок
                this._logger.LogWarning(error.Message);
            }
        };

        // Подключение к MQTT-брокеру и подписка на топик "esp8266/data"
        var response = await this._mqttClient.ConnectAsync();
        await this._mqttClient.SubscribeAsync("esp8266/data");

        // Цикл, выполняющийся до отмены токена
        while (!stoppingToken.IsCancellationRequested)
        {
            // Публикация сообщения "alive" на топик "esp8266/check"
            await this._mqttClient.PublishAsync("esp8266/check", "alive");
            // Задержка на 1 секунду перед следующим циклом
            await Task.Delay(1000);
        }
    }
}
