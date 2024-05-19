using IotTelemetry.Data;
using IotTelemetry.HostedServices;

namespace IotTelemetry
{
    internal class Startup : object
    {
        // Имя политики CORS
        public static readonly string CorsName = "WebUI";

        // Поле для хранения конфигурации
        private readonly IConfiguration configuration = default!;

        // Конструктор класса, инициализирует конфигурацию
        public Startup(IConfiguration configuration) : base()
        {
            this.configuration = configuration;
        }

        // Метод для настройки сервисов
        public void ConfigureServices(IServiceCollection services)
        {
            // Добавление контроллеров
            services.AddControllers();

            // Добавление возможностей для изучения конечных точек API
            services.AddEndpointsApiExplorer();

            // Добавление генератора Swagger
            services.AddSwaggerGen();

            // Добавление службы работы с базой данных
            services.AddDataBase();

            // Добавление пользовательской службы AvgService
            services.AddAvgService();

            // Настройка политики CORS
            services.AddCors(options => options.AddPolicy(Startup.CorsName, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod().AllowAnyHeader();
            }));

            // Добавление кэширования в памяти
            services.AddMemoryCache();

            // Добавление службы MQTT
            services.AddMqttService();
        }

        // Метод для настройки приложения
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            // Если среда разработки, включить Swagger и Swagger UI
            if (env.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }

            // Включение маршрутизации
            application.UseRouting();

            // Включение политики CORS
            application.UseCors(Startup.CorsName);

            // Настройка конечных точек для контроллеров
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}