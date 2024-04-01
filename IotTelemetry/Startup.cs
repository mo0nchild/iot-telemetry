
using IotTelemetry.Data;
using IotTelemetry.HostedServices;

namespace IotTelemetry
{
    internal class Startup : object
    {
        public static readonly string CorsName = "WebUI";
        private readonly IConfiguration configuration = default!;
        public Startup(IConfiguration configuration) : base() 
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDataBase();

            services.AddAvgService();

            services.AddCors(options => options.AddPolicy(Startup.CorsName, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod().AllowAnyHeader();
            }));
            services.AddMemoryCache();
            services.AddMqttService();
        }
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }
            application.UseRouting();
            application.UseCors(Startup.CorsName);
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
