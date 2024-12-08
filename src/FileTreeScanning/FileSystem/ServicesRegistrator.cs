using FileTreeScanning.Scanning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smusdi.Core.Extensibility;

namespace FileTreeScanning.FileSystem;

internal sealed class ServicesRegistrator : IServicesRegistrator
{
    public IServiceCollection Add(IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IScanStrategyPool<FileSystemScanOptions>, FileSystemScanStrategyPool>();
    }
}
