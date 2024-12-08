namespace FileTreeScanning.Scanning;

public interface IScanStrategyPool<T>
    where T : ScanOptions
{
    Task Init(T scanOptions);

    IScanStrategy GetOne();
}
