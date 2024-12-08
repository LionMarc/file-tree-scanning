using System.Text.Json;
using System.Threading.Channels;
using Serilog;

namespace FileTreeScanning.Scanning;

public sealed class JsonScanFilesPublisher(ILogger logger) : IScanFilesPublisher
{
    private readonly ILogger logger = logger.ForContext<JsonScanFilesPublisher>();

    public async Task Start(ChannelReader<ScanFile> scanFilesReader, CancellationToken cancellationToken)
    {
        var filesCount = 0;
        using var output = File.OpenWrite($"result_{Guid.NewGuid()}.json");
        var writer = new Utf8JsonWriter(output, new JsonWriterOptions
        {
            Indented = true,
        });
        writer.WriteStartArray();

        try
        {
            await foreach (var result in scanFilesReader.ReadAllAsync(cancellationToken))
            {
                filesCount++;
                writer.WriteStartObject();
                writer.WriteString(nameof(ScanFile.Type), result.Type.ToString());
                writer.WriteString(nameof(ScanFile.FullPath), result.FullPath);
                if (result.LastModificationTimestamp.HasValue)
                {
                    writer.WriteString(nameof(ScanFile.LastModificationTimestamp), result.LastModificationTimestamp.Value.ToString("O"));
                }

                if (result.Size.HasValue)
                {
                    writer.WriteNumber(nameof(ScanFile.Size), result.Size.Value);
                }

                writer.WriteEndObject();

                if (filesCount % 100 == 0)
                {
                    await writer.FlushAsync();
                }
            }
        }
        catch (OperationCanceledException e)
        {
            this.logger.Warning(e, "Processor has been cancelled.");
        }
        finally
        {
            writer.WriteEndArray();
            await writer.FlushAsync();

            var message = $"[JsonPublisher] {filesCount} files published.";
            this.logger.Information(message);
        }
    }
}
