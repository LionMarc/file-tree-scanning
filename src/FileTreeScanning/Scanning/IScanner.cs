namespace FileTreeScanning.Scanning;

public interface IScanner<T>
    where T : ScanOptions
{
    Task Scan(T options, CancellationToken cancellationToken);
}
