namespace FileTreeScanning.Scanning;

public class ScanOptions(string rootFolder)
{
    public const int DefaultScanWorkersCount = 10;

    public const string JsonPublisher = "JsonPublisher";

    public string RootFolder { get; } = rootFolder;

    public int ScanWorkersCount { get; set; } = DefaultScanWorkersCount;

    public string ScanFilesPublisher { get; set; } = JsonPublisher;
}
