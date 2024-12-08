namespace FileTreeScanning.Scanning;

public interface IScanStrategy
{
    IEnumerable<ScanFile> ScanFolder(string folder);
}
