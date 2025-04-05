using Microsoft.Extensions.DependencyInjection;

public class ServiceProviderContainer
{
    public IServiceProvider Provider { get; }
    public IServiceCollection Services { get; }

    public ServiceProviderContainer(IServiceCollection services, IServiceProvider provider)
    {
        Services = services;
        Provider = provider;
    }
}
