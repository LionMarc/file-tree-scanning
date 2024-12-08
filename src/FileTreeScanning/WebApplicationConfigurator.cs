using FileTreeScanning.FileSystem;
using FileTreeScanning.Scanning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smusdi.Core.Extensibility;

namespace FileTreeScanning;

internal sealed class WebApplicationConfigurator : IWebApplicationConfigurator
{
    public WebApplication Configure(WebApplication webApplication)
    {
        webApplication.MapGet("/scan_system_folder/{folder}", async (string folder, [FromQuery] int? workersCount, [FromServices] IScanner<FileSystemScanOptions> systemFileScanner) =>
        {
            var options = new FileSystemScanOptions(folder)
            {
                ScanWorkersCount = workersCount ?? ScanOptions.DefaultScanWorkersCount,
                ScanFilesPublisher = ScanOptions.JsonPublisher,
            };
            await systemFileScanner.Scan(options, CancellationToken.None);
            return Results.NoContent();
        });

        return webApplication;
    }
}
