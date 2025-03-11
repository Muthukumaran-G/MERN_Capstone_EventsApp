using Consul;

namespace WishlistService
{
    public static class ServiceRegistryExtensions
    {
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(op => new ConsulClient(c =>
            {
                c.Address = new Uri(configuration["ConsulConfig:ConsulAddress"]);
            }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var serviceId = $"{configuration["ConsulConfig:ServiceName"]}-{Guid.NewGuid()}";

            var registration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = configuration["ConsulConfig:ServiceName"],
                Address = configuration["ConsulConfig:ServiceHost"],
                Port = int.Parse(configuration["ConsulConfig:ServicePort"]),
                //Check = new AgentServiceCheck()
                //{
                //    HTTP = $"https://{configuration["ConsulConfig:ServiceHost"]}:{configuration["ConsulConfig:ServicePort"]}/health",
                //    Interval = TimeSpan.FromSeconds(10),
                //    Timeout = TimeSpan.FromSeconds(5)
                //}
            };

            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceId).ConfigureAwait(true);
            });

            return app;
        }
    }
}
