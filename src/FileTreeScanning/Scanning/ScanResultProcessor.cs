using System.Threading.Channels;
using Serilog;

namespace FileTreeScanning.Scanning;

public sealed class ScanResultProcessor(ILogger logger)
{
    private readonly ILogger logger = logger.ForContext<ScanResultProcessor>();

    public async Task Start(
        string rootFolder,
        ChannelWriter<string> folderWriter,
        ChannelReader<FolderScanResult> folderScanResultReader,
        ChannelWriter<ScanFile> scanFileWriter,
        CancellationToken cancellationToken)
    {
        var foldersCount = 0;

        try
        {
            var foldersInProgress = new HashSet<string>() { rootFolder };
            await folderWriter.WriteAsync(rootFolder, cancellationToken);

            await foreach (var result in folderScanResultReader.ReadAllAsync(cancellationToken))
            {
                if (result is FolderStatsResult)
                {
                    foldersCount++;
                    foldersInProgress.Remove(result.Folder);

                    if (foldersInProgress.Count == 0)
                    {
                        this.logger.Information("Scan completed");
                        folderWriter.Complete();
                        scanFileWriter.Complete();
                        return;
                    }
                }
                else if (result is ScanFileResult scanFileResult)
                {
                    if (scanFileResult.ScanFile.Type == ScanFileType.Folder)
                    {
                        foldersInProgress.Add(scanFileResult.ScanFile.FullPath);
                        await folderWriter.WriteAsync(scanFileResult.ScanFile.FullPath, cancellationToken);
                    }

                    await scanFileWriter.WriteAsync(scanFileResult.ScanFile, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException e)
        {
            this.logger.Warning(e, "Processor has been cancelled.");
        }
        finally
        {
            var message = $"{foldersCount} folders scanned.";
            this.logger.Information(message);
        }
    }
}
