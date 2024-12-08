using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smusdi.Core.Extensibility;

namespace FileTreeScanning.Scanning;

internal sealed class ServicesRegistrator : IServicesRegistrator
{
    public IServiceCollection Add(IServiceCollection services, IConfiguration configuration)
    {
        return services.AddScoped(typeof(IScanner<>), typeof(Scanner<>))
            .AddKeyedScoped<IScanFilesPublisher, JsonScanFilesPublisher>(ScanOptions.JsonPublisher);
    }
}
