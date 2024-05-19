namespace IotTelemetry
{
    // Определение внутреннего класса Program
    internal class Program : object
    {
        // Конструктор класса Program, вызывающий базовый конструктор
        public Program() : base() { }

        // Точка входа в приложение
        public static void Main(string[] args)
            // Создание и запуск хоста приложения
            => CreateHostBuilder(args).Build().Run();

        // Метод для создания и настройки хоста приложения
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Создание стандартного хоста с настройками по умолчанию
            return Host.CreateDefaultBuilder(args)
                // Конфигурация веб-хоста с использованием класса Startup
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
