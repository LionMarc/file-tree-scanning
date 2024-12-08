namespace FileTreeScanning.Scanning;

public sealed record ScanFileResult(string Folder, ScanFile ScanFile) : FolderScanResult(Folder);
