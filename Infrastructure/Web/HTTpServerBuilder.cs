using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class HttpServerBuilder
{
    public static IWebHost BuildHost(IServiceCollection services)
    {
        return WebHost
        .CreateDefaultBuilder()
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
        })
        .ConfigureServices(services =>
        {
            var originalServices = services.ToList();

            foreach (var serviceDescriptor in originalServices)
            {
                services.Add(serviceDescriptor);
            }

            services.AddControllers();
        })
        .Configure(app =>
        {
            app.UseRouting();

            app.UseCors(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        })
        .UseUrls("http://localhost:5000")
        .Build();

    }
}
