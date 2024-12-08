using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
using SerilogTimings;

namespace FileTreeScanning.Scanning;

public sealed class Scanner<T>(
    ILogger logger,
    IScanStrategyPool<T> scanStrategyPool,
    IServiceProvider serviceProvider) : IScanner<T>
    where T : ScanOptions
{
    public async Task Scan(T options, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(nameof(ScanOptions.RootFolder), options.RootFolder))
        using (Operation.Time("Scanning folder"))
        {
            var foldersChannel = Channel.CreateUnbounded<string>();
            var folderScanResultsChannel = Channel.CreateUnbounded<FolderScanResult>();
            var scanFilesChannel = Channel.CreateUnbounded<ScanFile>();

            await scanStrategyPool.Init(options);
            var workers = Enumerable.Range(0, options.ScanWorkersCount).Select((index) => new ScanWorker(index, logger, scanStrategyPool.GetOne())).ToList();
            workers.ForEach(worker => _ = worker.Start(
                foldersChannel.Reader,
                folderScanResultsChannel.Writer,
                cancellationToken));

            var scanFilesPublisher = serviceProvider.GetRequiredKeyedService<IScanFilesPublisher>(options.ScanFilesPublisher);
            _ = scanFilesPublisher.Start(scanFilesChannel.Reader, cancellationToken);

            var resultWriter = new ScanResultProcessor(logger);
            await resultWriter.Start(
                options.RootFolder,
                foldersChannel.Writer,
                folderScanResultsChannel.Reader,
                scanFilesChannel.Writer,
                cancellationToken);
        }
    }
}
