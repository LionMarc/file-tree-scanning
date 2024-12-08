using System.Threading.Channels;
using Serilog;

namespace FileTreeScanning.Scanning;

public sealed class ScanWorker(int index, ILogger logger, IScanStrategy scanStrategy)
{
    private readonly ILogger logger = logger.ForContext<ScanWorker>();

    public async Task Start(
        ChannelReader<string> folderReader,
        ChannelWriter<FolderScanResult> folderScanResultWriter,
        CancellationToken cancellationToken)
    {
        try
        {
            var scannedFolderCount = 0;
            await foreach (var folder in folderReader.ReadAllAsync(cancellationToken))
            {
                scannedFolderCount++;
                var message = $"[{index}] Scanning folder {folder}...";
                this.logger.Information(message);
                var filesCount = 0;
                foreach (var entry in scanStrategy.ScanFolder(folder))
                {
                    await folderScanResultWriter.WriteAsync(new ScanFileResult(folder, entry));
                    filesCount++;
                }

                await folderScanResultWriter.WriteAsync(new FolderStatsResult(folder, filesCount));

                message = $"[{index}] Folder {folder} scanned";
                this.logger.Information(message);
            }

            var info = $"[{index}] Scan completed. {scannedFolderCount} folders processed.";
            this.logger.Information(info);
        }
        catch (OperationCanceledException e)
        {
            var warning = $"[{index}] Worker has been cancelled.";
            this.logger.Warning(e, warning);
        }
    }
}
