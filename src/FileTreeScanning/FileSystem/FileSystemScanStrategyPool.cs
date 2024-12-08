using FileTreeScanning.Scanning;

namespace FileTreeScanning.FileSystem;

public sealed class FileSystemScanStrategyPool : IScanStrategyPool<FileSystemScanOptions>
{
    public IScanStrategy GetOne() => new FileSystemScanStrategy();

    public Task Init(FileSystemScanOptions scanOptions)
    {
        // There is nothing to do for file system
        return Task.CompletedTask;
    }
}
