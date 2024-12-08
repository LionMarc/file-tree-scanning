using System.Threading.Channels;

namespace FileTreeScanning.Scanning;

public interface IScanFilesPublisher
{
    Task Start(ChannelReader<ScanFile> scanFilesReader, CancellationToken cancellationToken);
}
